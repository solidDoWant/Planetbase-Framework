using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PlanetbaseFramework
{
    public abstract class ModBase
    {
        public List<Texture2D> modTextures { get; protected set; }
        public List<GameObject> modObjects { get; protected set; }
        public ModBase(string ModName = "")
        {
            if (ModName == null || ModName.Equals(""))
            {
                throw new ModNameNotSetException();
            }

            this.ModName = ModName;

            try
            {
                StringList.loadStringsFromFolder(this.ModPath, this.ModName, StringList.mStrings);
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't load string file for the mod " + this.ModName + " because of exception: " + e.ToString() + ": " + e.Message);
            }

            try
            {
                modTextures = this.LoadAllPNG("assets" + Path.DirectorySeparatorChar + "png" + Path.DirectorySeparatorChar);

            }
            catch (Exception)
            {
                Debug.Log("Couldn't/no PNG files to load");
            }

            try
            {
                modObjects = this.LoadAllOBJ("assets" + Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar);

            }
            catch (Exception)
            {
                Debug.Log("Couldn't/no OBJ files to load");
            }
        }

        public string ModName
        {
            get; set;
        }

        public static string BasePath
        {
            get
            {
                return Util.getFilesFolder() + Path.DirectorySeparatorChar.ToString() + "Mods" + Path.DirectorySeparatorChar.ToString();
            }
        }

        public string ModPath
        {
            get
            {
                return BasePath + this.ModName + Path.DirectorySeparatorChar.ToString();
            }
        }

        public virtual void Init()
        {
        }

        public virtual void Update()
        {

        }

        private class ModNameNotSetException : Exception
        {
            public override string Message
            {
                get
                {
                    return "A mod's name is either null or undefined.";
                }
            }
        }

        public List<Texture2D> LoadAllPNG(string subfolder = null)
        {
            string[] files = GetFilesMatchingFiletype("png", subfolder);

            List<Texture2D> loadedFiles = new List<Texture2D>(files.Length);
            foreach (String file in files)
            {
                loadedFiles.Add(Utils.LoadPNGFromFile(file));
            }

            return loadedFiles;
        }

        public List<GameObject> LoadAllOBJ(string subfolder = null)
        {

            string[] files = GetFilesMatchingFiletype("obj", subfolder);

            List<GameObject> loadedFiles = new List<GameObject>(files.Length);
            foreach (String file in files)
            {
                GameObject loadedObject = OBJLoader.LoadOBJFile(file, modTextures);
                loadedObject.setVisibleRecursive(false);
                loadedObject.name = Path.GetFileName(file);
                loadedFiles.Add(loadedObject);
            }

            return loadedFiles;
        }

        private string[] GetFilesMatchingFiletype(string filetype, string subfolder = null)
        {
            if (subfolder == null)
            {
                return Directory.GetFiles(this.ModPath, "*." + filetype);
            }
            else if (Directory.Exists(this.ModPath + subfolder))
            {
                return Directory.GetFiles(this.ModPath + subfolder);
            }
            else
            {
                throw new Exception("Could not load " + filetype.ToUpper() + " files from invalid folder " + this.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
            }
        }
    }
}
