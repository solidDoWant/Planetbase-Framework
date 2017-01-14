using Planetbase;
using System;
using System.IO;
using UnityEngine;

namespace PlanetbaseFramework
{
    public abstract class ModBase
    {
        public Texture2D[] modTextures
        {
            get; set;
        }
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

            } catch(Exception)
            {
                Debug.Log("Couldn't/no PNG files to load");
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

        public abstract void Init();

        public abstract void Update();

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

        public Texture2D LoadPNG(string filename)
        {
            string path = this.ModName + filename;
            return Utils.LoadPNGFromFile(path);
        }

        public Texture2D[] LoadAllPNG(string subfolder = null)
        {
            string[] files = null;
            if (subfolder == null)
            {
                files = Directory.GetFiles(this.ModPath, "*.png");
            }
            else if (Directory.Exists(this.ModPath + subfolder + Path.DirectorySeparatorChar.ToString()))
            {
                files = Directory.GetFiles(this.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
            }
            else
            {
                Debug.Log("Could not load PNG files from invalid folder " + this.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
                throw new Exception("Could not load PNG files from invalid folder " + this.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
            }

            Texture2D[] loadedFiles = new Texture2D[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                loadedFiles[i] = LoadPNG(subfolder + Path.DirectorySeparatorChar.ToString() + files[i]);
            }

            return loadedFiles;
        }
    }
}
