﻿using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using ICSharpCode.SharpZipLib.Zip;
using PlanetbaseFramework.Patches.Planetbase.GameStateCredits;
using PlanetbaseFramework.Patches.Planetbase.GameStateTitle;
using PlanetbaseFramework.Patches.Planetbase.Util;
using UnityEngine;

namespace PlanetbaseFramework
{
    public abstract class ModBase
    {
        public List<Texture2D> ModTextures { get; protected set; }
        public List<GameObject> ModObjects { get; protected set; }

        public virtual Version ModVersion => new Version(0, 0, 0, 0);

        private HarmonyInstance Harmony { get; set; }

        private static FastZip ZipInstance { get; } = new FastZip();

        protected ModBase()
        {
            //Extract embedded assets
            ZipConstants.DefaultCodePage = 0;   //This is a workaround to get files to extract properly

            var currentAssembly = Assembly.GetCallingAssembly();
            var manifest = currentAssembly.GetManifestResourceNames();

            PreProcessEmbeddedResources(manifest);

            foreach (var file in manifest)
            {
                if (!PreProcessEmbeddedResource(file)) continue;

                Debug.Log($"Processing embedded file \"{file}\"");

                using (var resourceStream = currentAssembly.GetManifestResourceStream(file))
                {
                    switch (Path.GetExtension(file))
                    {
                        case ".zip":
                            Debug.Log("zip " + GetResourceRelativeFilePath(file));
                            ZipInstance.ExtractZip(
                                resourceStream,
                                ModPath,
                                FastZip.Overwrite.Always,
                                null,
                                null,
                                null,
                                false,
                                false
                            );
                            break;
                        default:    //Copy the file to a directory matching the name under the mod's folder
                            var filePath = Path.Combine(ModPath, GetResourceRelativeFilePath(file));

                            Debug.Log($"Loading \"{file}\" to \"{filePath}\"");

                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                            using (var fileStream = File.Create(filePath))
                            {
                                resourceStream.CopyTo(fileStream);
                            }

                            break;
                    }
                }
            }

            try
            {
                LoadAllStrings("strings");
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load strings files due to exception:");
                Utils.LogException(e);
            }

            try
            {
                ModTextures = LoadAllPngs("png");

                if (ModTextures.Count > 0)
                {
                    Debug.Log($"Successfully loaded {ModTextures.Count} texture(s)");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load PNG files due to exception:");
                Utils.LogException(e);
            }

            try
            {
                ModObjects = LoadAllObjs("obj");

                if(ModObjects.Count > 0)
                {
                    Debug.Log($"Successfully loaded {ModObjects.Count} object(s)");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load OBJ files due to exception:");
                Utils.LogException(e);
            }

            var credits = GetCredits().Trim();
            if (!string.IsNullOrEmpty(credits))
                ConstructorPatch.Credits[this] = credits;
        }

        public abstract string ModName { get; }

        public static string BasePath { get; } = Path.Combine(FilesFolderPatch.FilesFolderPath, "Mods");

        public virtual string ModPath => Path.Combine(BasePath, ModName);

        public virtual string AssetsPath => Path.Combine(ModPath, "assets");

        public virtual void Init()  //This is virtual instead of abstract so mods aren't required to implement it. Same with Update below
        {
        }

        public virtual void Update()
        {
        }

        public int LoadAllStrings(string subfolder = null)
        {
            var files = GetAssetsMatchingFileType("xml", subfolder);

            Debug.Log($"Found {files.Length} strings files");

            foreach (var file in files)
            {
                Utils.LoadStringsFromFile(file);
            }

            return files.Length;
        }

        public List<Texture2D> LoadAllPngs(string subfolder = null)
        {
            var files = GetAssetsMatchingFileType("png", subfolder);

            Debug.Log($"Found {files.Length} PNG files");

            var loadedFiles = new List<Texture2D>(files.Length);
            foreach (var file in files)
            {
                loadedFiles.Add(Utils.LoadPngFromFile(file));
            }

            return loadedFiles;
        }

        public List<GameObject> LoadAllObjs(string subfolder = null)
        {
            var files = GetAssetsMatchingFileType("obj", subfolder);

            Debug.Log($"Found {files.Length} OBJ files");

            var loadedFiles = new List<GameObject>(files.Length);
            foreach (var file in files)
            {
                var loadedObject = ObjLoader.LoadOBJFile(file, ModTextures);
                loadedObject.setVisibleRecursive(false);
                loadedObject.name = Path.GetFileName(file);
                loadedObject.tag = "Untagged";
                loadedFiles.Add(loadedObject);
            }

            return loadedFiles;
        }

        private string[] GetAssetsMatchingFileType(string fileType, string subfolder = null)
        {
            if (subfolder == null)
                subfolder = string.Empty;

            var searchPath = Path.Combine(AssetsPath, subfolder);

            return Directory.Exists(searchPath) ? Directory.GetFiles(searchPath, "*." + fileType) : new string[0];
        }

        public HarmonyInstance GetHarmonyInstance() => Harmony ?? (Harmony = HarmonyInstance.Create(ModName));

        public void InjectPatches()
        {
            GetHarmonyInstance().PatchAll(Assembly.GetCallingAssembly());
        }

        public void InjectPatches(Assembly containingPatches)
        {
            GetHarmonyInstance().PatchAll(containingPatches);
        }

        public void RegisterTitleButton(TitleButton button)
        {
            TitleButtonPatch.RegisteredTitleButtons.Add(button);
        }

        /// <summary>
        /// Do any pre-load actions before embedded resources are loaded, such as removing existing folders/items.
        /// Warning: this call will be made in the constructor before child initialization.
        /// </summary>
        /// <param name="resourceNames">The name of all the resources to be loaded</param>
        protected virtual void PreProcessEmbeddedResources(string[] resourceNames)
        {

        }

        /// <summary>
        /// Do any pre-load actions before an embedded resource is loaded, such as removing existing folders/items.
        /// Warning: this call will be made in the constructor before child initialization.
        /// </summary>
        /// <param name="resourceName">The name of the resources being loaded</param>
        /// <returns>True if the resource should be loaded, false otherwise</returns>
        protected virtual bool PreProcessEmbeddedResource(string resourceName)
        {
            return true;
        }

        // Resource names will be of the format:
        // <ProjectName>.some.directory.path.fileName.extension
        // For example:
        // * PlanetbaseFramework.assets.png.error.png
        // * PlanetbaseFramework.LICENSE.txt
        // This code does not currently support files without an extension.
        private static string GetResourceRelativeFilePath(string resourceName)
        {
            //Remove the project name from the path, including the preceding '.'
            var convertedFilePath = resourceName.Substring(resourceName.IndexOf('.') + 1);

            //Replace the '.' characters for directories with the path separation character
            convertedFilePath = convertedFilePath.Substring(0, convertedFilePath.LastIndexOf('.'))
                .Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(convertedFilePath);

            return convertedFilePath;
        }

        /// <summary>
        /// This function updates the "Credits" screen with the return value.
        /// </summary>
        public virtual string GetCredits()
        {
            var authors = GetContributors();
            if (authors == null)
                return null;

            if (authors.Count == 0)
                return null;

            var headerString = $"(h){ModName}(/h)";
            var authorsString = string.Join("\n", authors.ToArray());
            return headerString + "\n" + authorsString;
        }

        /// <summary>
        /// Called by the default implementation of GetCredits to add the returned
        /// authors to the credit screen.
        /// </summary>
        public virtual ICollection<string> GetContributors()
        {
            return new string[] { };
        }
    }
}