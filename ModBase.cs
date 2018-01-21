using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PlanetbaseFramework
{
    public abstract class ModBase
    {
        public List<Texture2D> ModTextures { get; protected set; }
        public List<GameObject> ModObjects { get; protected set; }

        public virtual Version ModVersion => new Version(0, 0, 0, 0);

        protected ModBase()
        {
            try
            {
                LoadAllString(Path.Combine("assets", "strings"));
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load strings files due to exception:");
                Utils.LogException(e);
            }

            try
            {
                ModTextures = LoadAllPng(Path.Combine("assets","png"));

                if (ModTextures.Count > 0)
                {
                    Debug.Log("Successfully loaded " + ModTextures.Count + " texture(s)");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load PNG files due to exception:");
                Utils.LogException(e);
            }

            try
            {
                ModObjects = LoadAllObj(Path.Combine("assets", "obj"));

                if(ModObjects.Count > 0)
                {
                    Debug.Log("Successfully loaded " + ModObjects.Count + " object(s)");
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load OBJ files due to exception:");
                Utils.LogException(e);
            }
        }

        public abstract string ModName { get; }

        //Some of you might notice the odd '/' character in this string. This is because native PB code doesn't use Path.DirectorySeparatorChar, causing
        //one char to be wrong. I'll fix it at some point after I rewrite the patcher.
        public static string BasePath => Util.getFilesFolder() + Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar;

        public virtual string ModPath => BasePath + ModName + Path.DirectorySeparatorChar;

        public virtual void Init()  //This is virtual instead of abstract so mods aren't required to implement it. Same with Update below
        {
        }

        public virtual void Update()
        {
        }

        public int LoadAllString(string subfolder = null)
        {
            string[] files = GetFilesMatchingFiletype("xml", subfolder);

            Debug.Log("Found " + files.Length + " strings files");

            foreach (string file in files)
            {
                Utils.LoadStringsFromFile(file);
            }

            return files.Length;
        }

        public List<Texture2D> LoadAllPng(string subfolder = null)
        {
            string[] files = GetFilesMatchingFiletype("png", subfolder);

            Debug.Log("Found " + files.Length + " PNG files");

            List<Texture2D> loadedFiles = new List<Texture2D>(files.Length);
            foreach (String file in files)
            {
                loadedFiles.Add(Utils.LoadPngFromFile(file));
            }

            return loadedFiles;
        }

        public List<GameObject> LoadAllObj(string subfolder = null)
        {
            string[] files = GetFilesMatchingFiletype("obj", subfolder);

            Debug.Log("Found " + files.Length + " OBJ files");

            List<GameObject> loadedFiles = new List<GameObject>(files.Length);
            foreach (String file in files)
            {
                GameObject loadedObject = ObjLoader.LoadOBJFile(file, ModTextures);
                loadedObject.setVisibleRecursive(false);
                loadedObject.name = Path.GetFileName(file);
                loadedObject.tag = "Untagged";
                loadedFiles.Add(loadedObject);
            }

            return loadedFiles;
        }

        private string[] GetFilesMatchingFiletype(string filetype, string subfolder = null)
        {
            if (subfolder == null)
            {
                subfolder = string.Empty;
            }

            return Directory.Exists(ModPath + subfolder) ? Directory.GetFiles(ModPath + subfolder, "*." + filetype) : new string[0];
        }
    }
}