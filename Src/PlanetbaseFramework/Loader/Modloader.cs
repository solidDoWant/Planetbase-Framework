using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PlanetbaseFramework
{
    /*
     * This is the core class behind loading all mods. The patcher injects calls to loadMods() and updateMods() into the native PB code, which then
     * calls the methods in this file. This allows for minimal changes to PB's native code, while still allowing it to be extended.
     */
    public static class ModLoader
    {
        public static List<ModBase> InitializedMods = new List<ModBase>();

        /// <summary>
        /// Called by the game manager on startup to load in mods
        /// </summary>
        public static void LoadMods()
        {
            Debug.Log("Loading mods...");

            // Split to function: LoadAssemblyFiles
            var modDLLs = new List<string>();

            modDLLs.Add(Assembly.GetExecutingAssembly().Location);

            if (Directory.Exists(ModBase.BasePath))
            {
                modDLLs.AddRange(Directory.GetFiles(ModBase.BasePath, "*.dll"));
            }
            else
            {
                Debug.Log($"Mod directory does not exist, creating at \"{ModBase.BasePath}\"");
                Directory.CreateDirectory(ModBase.BasePath);

                //Create the planetbase mod folder and extract the assets
                
            }

            Debug.Log($"Found {modDLLs.Count} mods");
            
            foreach (var file in modDLLs)
            {
                // Split to function- LoadTypes()
                Type[] types;
                try
                {
                    types = Assembly.LoadFile(file).GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    Utils.LogException(e);
                    foreach (var loaderException in e.LoaderExceptions)
                    {
                        Utils.LogException(loaderException);
                    }

                    Debug.Log(
                        "************************ Note to modders: If you're seeing this exception, you probably are using a a post .Net 2.0.5.0 function.\r\n" +
                        "For convenience I've made it so you can use mods compiled after 2.0.5.0, however modern features are not available. ************************"
                    );

                    continue;
                }
                catch (Exception e)
                {
                    Utils.LogException(e);
                    continue;
                }

                // Create method- InjectMods(types)
                foreach (var type in types)
                {
                    //Skip if the type isn't a mod
                    if (!typeof(ModBase).IsAssignableFrom(type) || type.IsAbstract || !type.IsPublic ||
                        Attribute.IsDefined(type, typeof(ModLoaderIgnoreAttribute))) continue;

                    var typeName = type.Name;

                    Debug.Log($"Loading mod \"{typeName}\" from file \"{file}\"");

                    ModBase mod = null;
                    try
                    {
                        mod = Activator.CreateInstance(type) as ModBase;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(
                            $"Error loading mod from file: \"{file}\" of type: \"{typeName}\". Exception thrown:");
                        Utils.LogException(e);
                    }

                    if (mod != null)
                    {
                        var modName = mod.ModName;

                        try
                        {
                            mod.Init();
                            ModList.Add(mod);
                            Debug.Log($"Loaded mod \"{modName}\"");
                        }
                        catch (Exception e)
                        {
                            Debug.Log(
                                $"Error initializing mod \"{modName}\" from file: \"{file}\" of type: {typeName}"
                            );
                            Utils.LogException(e);
                        }
                    }
                    else
                    {
                        Debug.Log($"Failed to load mod \"{typeName}\" from file \"{file}\"");
                    }
                }
            }

            Debug.Log($"Successfully loaded {modDLLs.Count} mods");
        }

        /// <summary>
        /// Update the mods in the order they were loaded on each game tick.
        /// </summary>
        public static void UpdateMods()
        {
            foreach(var mod in ModList)
            {
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
        }

        /// <summary>
        /// Utility method to get mods that match the provided type
        /// </summary>
        /// <typeparam name="T">The type of the mod to look for</typeparam>
        /// <returns>A list of instantiated mods matching the type</returns>
        public static List<T> GetModByType<T>() where T : ModBase => GetModByType(typeof(T)).Cast<T>().ToList();

        /// <summary>
        /// Utility method to get mods that match the provided type
        /// </summary>
        /// <param name="modType">The type of the mod to look for</param>
        /// <returns>A list of instantiated mods matching the type</returns>
        public static List<ModBase> GetModByType(Type modType)
        {
            var matchedMods = new List<ModBase>();    //oh boy, sure wish I could use linq right about now

            foreach (var mod in ModList)
            {
                if (mod.GetType() == modType/*.Compare(modType)*/)
                {
                    matchedMods.Add(mod);
                }
            }

            return matchedMods;
        }
    }
}