using Planetbase;
using System;
using System.IO;
using UnityEngine;

namespace PlanetbaseFramework
{
    public static class Utils
    {
        public static Texture2D LoadPNGFromFile(string AbsolutePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(AbsolutePath))
            {
                fileData = File.ReadAllBytes(AbsolutePath);
                tex = new Texture2D(2, 2);  //TODO fix this to be of arbitrary size
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }

        public static bool Compare (this Type t1, Type t2)
        {
            return t1.FullName.Equals(t2.FullName);
        }
    }
}
