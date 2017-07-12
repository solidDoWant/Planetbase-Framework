using Planetbase;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PlanetbaseFramework
{
    public static class Utils
    {   public static Texture2D ErrorTexture { get; internal set; }

        public static Texture2D LoadPNGFromFile(string AbsolutePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(AbsolutePath))
            {
                fileData = File.ReadAllBytes(AbsolutePath);
                tex = new Texture2D(2, 2);  //TODO fix this to be of arbitrary size
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                tex.name = Path.GetFileName(AbsolutePath);
            }
            else
            {
                Debug.Log("Error loading texture: \"" + AbsolutePath + "\"");
                tex = ErrorTexture;
            }
            return tex;
        }

        //This should be called on any normal maps either at the init stage or the constructor of the mod
        public static void SetNormalMap(this Texture2D tex)
        {
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                Color temp = pixels[i];
                temp.r = pixels[i].g;
                temp.a = pixels[i].r;
                pixels[i] = temp;
            }
            tex.SetPixels(pixels);
        }

        public static T FindObjectByFilename<T>(this List<T> List, string Filename) where T : UnityEngine.Object
        {
            try
            {
                return List.Find(x => x.name == Filename);
            }
            catch (Exception e)
            {
                Debug.Log("Error loading file: \"" + Filename + "\" with type: \"" + typeof(T).ToString());
                Debug.Log("Stacktrace: ");
                Debug.Log(e.ToString());

                return null;
            }
        }

        public static T FindObjectByFilepath<T>(this List<T> List, string Filepath) where T : UnityEngine.Object
        {
            return FindObjectByFilename(List, Path.GetFileName(Filepath));
        }

        public static bool isValidTag(this string toCheck)
        {
            try
            {
                GameObject.FindGameObjectsWithTag(toCheck);
                return true;
            } catch(UnityException)
            {
                return false;
            }
        }

        public static bool Compare (this Type t1, Type t2)
        {
            return t1.FullName.Equals(t2.FullName);
        }
    }
}
