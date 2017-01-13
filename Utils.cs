using Planetbase;
using System;
using System.IO;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class Utils
    {
        public static Texture2D LoadPNG(ModBase mod, string filename)
        {
            string path = mod.ModPath + filename;
            return LoadPNGFromFile(path);
        }

        public static Texture2D[] LoadAllPNG(ModBase mod, string subfolder = null)
        {
            string[] files = null;
            if(subfolder == null)
            {
                files = Directory.GetFiles(mod.ModPath, "*.png");
            } else if(Directory.Exists(mod.ModPath + subfolder + Path.DirectorySeparatorChar.ToString()))
            {
                files = Directory.GetFiles(mod.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
            }
            else
            {
                Debug.Log("Could not load PNG files from invalid folder " + mod.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
                throw new Exception("Could not load PNG files from invalid folder " + mod.ModPath + subfolder + Path.DirectorySeparatorChar.ToString());
            }

            Texture2D[] loadedFiles = new Texture2D[files.Length];
            for(int i = 0; i < files.Length; i++)
            {
                loadedFiles[i] = LoadPNG(mod, subfolder + Path.DirectorySeparatorChar.ToString() + files[i]);
            }

            return loadedFiles;
        }

        public static Texture2D LoadPNGFromFile(string AbsolutePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(AbsolutePath))
            {
                fileData = File.ReadAllBytes(AbsolutePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }
    }
}
