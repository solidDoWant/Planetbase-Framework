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

        protected ModBase()
        {
            try
            {
                Debug.Log("Loaded " + LoadAllString("assets" + Path.DirectorySeparatorChar + "strings" + Path.DirectorySeparatorChar) + " string file(s)");
            }
            catch (Exception)
            {
                Debug.Log("Couldn't/no strings filies to load");
            }

            try
            {
                ModTextures = LoadAllPng("assets" + Path.DirectorySeparatorChar + "png" + Path.DirectorySeparatorChar);
            }
            catch (Exception)
            {
                Debug.Log("Couldn't/no PNG files to load");
            }

            try
            {
                ModObjects = LoadAllObj("assets" + Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar);

            }
            catch (Exception)
            {
                Debug.Log("Couldn't/no OBJ files to load");
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

            foreach (string file in files)
            {
                Utils.LoadStringsFromFile(file);
            }

            return files.Length;
        }

        public List<Texture2D> LoadAllPng(string subfolder = null)
        {
            string[] files = GetFilesMatchingFiletype("png", subfolder);

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

            if (Directory.Exists(this.ModPath + subfolder))
            {
                return Directory.GetFiles(this.ModPath + subfolder, "*." + filetype);
            }
            else
            {
                throw new Exception("Could not load " + filetype.ToUpper() + " files from invalid folder " + this.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
            }
        }
    }
}