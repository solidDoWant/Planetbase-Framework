using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Mono.Cecil;
using Planetbase;
using PlanetbaseFramework.Cecil;
using UnityEngine;

namespace PlanetbaseFramework
{
    /*
     * This is the core class behind loading all mods. The patcher injects calls to LoadMods(), which then calls the
     * methods in this file. This allows for minimal changes to PB's native code, while still allowing it to be extended.
     * Further changes to the base game's code can be implemented with Harmony.
     */
    public class ModLoader
    {
        /// <summary>
        ///     A list of all mods that have been initialized
        /// </summary>
        public static List<ModBase> ModList { get; } = new List<ModBase>();

        protected static PerformanceTimer LoadTimer { get; } = new PerformanceTimer("Mod loading");

        /// <summary>
        ///     Called by the game manager on startup to load in mods
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public static void LoadMods()
        {
            LoadTimer.start();
            Debug.Log("Planetbase Framework mod loading stage started...");

            SetupPrerequisites();

            var modDLLs = GetModCandidates();
            var totalAttemptedModCount = modDLLs.Sum(ProcessModCandidate);

            Debug.Log($"Successfully loaded {ModList.Count} of {totalAttemptedModCount} mods");
            if (modDLLs.Count > totalAttemptedModCount)
                Debug.Log(
                    "Note: Additional mods may have been loaded by second stage mod loaders (i.e. compatibility layers)");

            LoadTimer.stop();
            Debug.Log($"Mod loading took {LoadTimer.formatTime(LoadTimer.getLastMicros())}");
        }

        /// <summary>
        ///     Attempts to load and initialize mods found in a DLL.
        /// </summary>
        /// <param name="dllFilePath">The path to a DLL that may contain mods</param>
        /// <returns>The number of mods that were found. This may not be the number of mods successfully loaded.</returns>
        protected static int ProcessModCandidate(string dllFilePath)
        {
            var dllModTypes = new LinkedList<TypeDefinition>(FindDllMods(dllFilePath));
            if (!dllModTypes.Any())
                return 0;

            Debug.Log($"Found mod(s) in \"{dllFilePath}\". Loading assembly...");
            var modAssembly = Utils.LoadAssembly(dllFilePath);
            if (modAssembly == null)
                return 0;

            var successfullyLoadedModCount = dllModTypes
                .Select(modType => LoadMod<ModBase>(modAssembly, modType.FullName))
                .Where(loadedMod => loadedMod != null)
                .Select(InitializeMod)
                .Count();

            Debug.Log(
                $"Successfully loaded {successfullyLoadedModCount} out of {dllModTypes.Count} mods from \"{dllFilePath}\"");
            return dllModTypes.Count;
        }

        /// <summary>
        ///     This is the very first place where PB framework code is hit by the game's code, so
        ///     there are a few things not strictly related to mod loading that must be setup here.
        /// </summary>
        protected static void SetupPrerequisites()
        {
            // Ensures that assemblies and their dependencies are resolved properly
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            EnsureModDirectoryExists();
        }

        protected static bool InitializeMod(ModBase mod)
        {
            try
            {
                mod.Init();
                ModList.Add(mod);
                Debug.Log($"Loaded mod \"{mod.ModName}\"");
            }
            catch (Exception e)
            {
                Debug.Log($"Error initializing mod \"{mod.ModName}\"");
                Utils.LogException(e);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Loads and instantiates the a mod of a given type from a given assembly.
        ///     This does not initialize the mod, only instantiates the type.
        /// </summary>
        /// <param name="modAssembly">The assembly containing the provided type</param>
        /// <param name="modTypeName">The name of the type to load.</param>
        /// <returns>The loaded mod if successful, null otherwise.</returns>
        public static T LoadMod<T>(Assembly modAssembly, string modTypeName) where T : ModBase
        {
            var assemblyName = modAssembly.GetName().Name;
            Debug.Log($"Loading mod of type \"{modTypeName}\" from assembly \"{assemblyName}\"");

            T mod = null;
            try
            {
                var modType = modAssembly.GetType(modTypeName);
                var constructor = modType.GetConstructor(Type.EmptyTypes) ??
                                  throw new Exception(
                                      $"The type \"{modType.FullName}\" does not have a parameterless constructor");

                mod = FormatterServices.GetUninitializedObject(modType) as T;

                if (mod == null)
                    throw new Exception(
                        $"The type \"{modType.FullName}\" is not convertible to \"{typeof(T).FullName}\".");

                // Populate the mod's fields and call constructor
                // This is safe to assume as non-null as the property name is checked at compile time (if mod.ModAssembly does not exist, then compilation fails)
                modType.GetProperty(nameof(mod.ModAssembly)).SetValue(mod, modAssembly, null);
                constructor.Invoke(mod, null);

                Debug.Log($"Instantiated mod \"{mod.ModName}\" from type \"{modTypeName}\"");
            }
            catch (Exception e)
            {
                Debug.Log(
                    $"Error loading mod from assembly: \"{assemblyName}\" of type: \"{modTypeName}\"");
                Utils.LogException(e);
            }

            return mod;
        }

        protected static IEnumerable<TypeDefinition> FindDllMods(string filePath)
        {
            Debug.Log($"Checking \"{filePath}\" for Planetbase Framework compatible mods...");
            return ModuleLoader.LoadByPath(filePath).Types.Where(IsTypeDefinitionValidMod);
        }

        public static bool IsTypeDefinitionValidMod(TypeDefinition checkingType)
        {
            if (!checkingType.IsClass)
                return false;

            if (checkingType.IsNotPublic)
                return false;

            if (checkingType.IsAbstract)
                return false;

            if (checkingType.HasIgnoreAttribute())
                return false;

            if (!checkingType.HasTypeAsParent(typeof(ModBase).FullName))
                return false;

            return true;
        }

        protected static void EnsureModDirectoryExists()
        {
            if (Directory.Exists(ModBase.BasePath))
                return;

            Debug.Log($"Mod directory does not exist, creating at \"{ModBase.BasePath}\"");
            Directory.CreateDirectory(ModBase.BasePath);
        }

        protected static List<string> GetModCandidates()
        {
            var modDLLs = new List<string>();
            modDLLs.Add(Assembly.GetExecutingAssembly().Location);
            modDLLs.AddRange(Directory.GetFiles(ModBase.BasePath, "*.dll"));

            Debug.Log($"Found {modDLLs.Count} mod candidates");

            return modDLLs;
        }

        /// <summary>
        ///     Update the mods in the order they were loaded on each game tick.
        /// </summary>
        public static void UpdateMods()
        {
            foreach (var mod in ModList)
                try
                {
                    mod.Update();
                }
                catch (Exception e)
                {
                    Debug.Log($"Error updating mod {mod.ModName}");
                    Utils.LogException(e);
                }
        }

        /// <summary>
        ///     Utility method to get mods that match the provided type
        /// </summary>
        /// <typeparam name="T">The type of the mod to look for</typeparam>
        /// <returns>The instantiated mods matching type <c>T</c></returns>
        // ReSharper disable once UnusedMember.Global
        public static IEnumerable<T> GetModByType<T>() where T : ModBase
        {
            return GetModByType(typeof(T)).Cast<T>();
        }

        /// <summary>
        ///     Utility method to get mods that match the provided type
        /// </summary>
        /// <param name="modType">The type of the mod to look for</param>
        /// <returns>The instantiated mods matching the type</returns>
        public static IEnumerable<ModBase> GetModByType(Type modType)
        {
            return ModList.Where(mod => mod.GetType() == modType);
        }

        protected static Assembly CurrentDomain_AssemblyResolve(object source, ResolveEventArgs e)
        {
            return Utils.LoadAssembly(ModuleLoader.LoadByAssemblyName(e.Name).FullyQualifiedName);
        }
    }
}