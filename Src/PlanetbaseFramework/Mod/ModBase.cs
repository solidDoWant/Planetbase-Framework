using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Harmony;
using ICSharpCode.SharpZipLib.Zip;
using PlanetbaseFramework.Patches.Planetbase.GameStateTitle;
using UnityEngine;
using System.ComponentModel;

namespace PlanetbaseFramework
{
    public abstract class ModBase
    {
        public abstract string ModName { get; }

        public virtual Version ModVersion => new Version(0, 0, 0, 0);
        public virtual string ModPath => Path.Combine(BasePath, ModName);
        public virtual string AssetsPath => Path.Combine(ModPath, "assets");

        public List<Texture2D> ModTextures { get; protected set; }
        public List<GameObject> ModObjects { get; protected set; }

        //Some of you might notice the odd '/' character in this string. This is because native PB code doesn't use Path.DirectorySeparatorChar, causing
        //one char to be wrong. I'll fix it at some point after I rewrite the patcher.
        public static string BasePath { get; } = Path.Combine(Util.getFilesFolder(), "Mods");

        private static IResourceUnpacker _resourceUnpacker; 

        private HarmonyInstance Harmony { get; set; }


        protected ModBase()
        {
            // Consider moving out.
            ExtractAssets();
            LoadStrings();
            LoadTextures();
            LoadMeshes();
            SetupUnpackers();
        }

        /// <summary>
        /// This is virtual instead of abstract so mods aren't required to implement it. Same with Update below
        /// </summary>
        public virtual void Init()  
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

        private static void SetupUnpackers()
        {
            var resourceUnpacker = new ResourceUnpacker();
            
            var zipUnpacker = new ZipUnpacker();
            resourceUnpacker.RegisterUnpacker(zipUnpacker);

            var defaultUnpacker = new NoUnpacker();
            resourceUnpacker.RegisterDefault(defaultUnpacker);
        }

        private static void ExtractAssets()
        {
            var currentAssembly = Assembly.GetCallingAssembly();
            var manifest = currentAssembly.GetManifestResourceNames();

            PreProcessEmbeddedResources(manifest);

            foreach (var file in manifest)
            {
                if (!PreProcessEmbeddedResource(file)) continue;

                Debug.Log($"Processing embedded file \"{file}\"");
            }

            _resourceUnpacker.Unpack(re)
        }

        private static void LoadStrings()
        {
            // LoadStrings
            try
            {
                LoadAllStrings("strings");
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load strings files due to exception:");
                Utils.LogException(e);
            }
        }

        private static void LoadTextures()
        {
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
        }

        private static void LoadMeshes()
        {
            try
            {
                ModObjects = LoadAllObjs("obj");

                if (ModObjects.Count > 0)
                {
                    Debug.Log($"Successfully loaded {ModObjects.Count} object(s)");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load OBJ files due to exception:");
                Utils.LogException(e);
            }
        }

        private static string GetResourceRelativeFilePath(string resourceName)
        {
            //Remove the project name from the path, including the preceding '.'
            var convertedFilePath = resourceName.Substring(resourceName.IndexOf('.') + 1);

            //Replace the '.' characters for directories with the path separation character
            convertedFilePath = convertedFilePath.Substring(0, convertedFilePath.LastIndexOf('.'))
                .Replace('.', Path.DirectorySeparatorChar) + Path.GetExtension(convertedFilePath);

            return convertedFilePath;
        }

        private class SomethingFastZip : FastZip
        {
            public SomethingFastZip()
            {
                //This is a workaround to get files to extract properly.
                // Define properly.
                // Don't inline comments.
                ZipConstants.DefaultCodePage = 0;
            }
        }

    }
}