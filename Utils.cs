using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework
{
    public static class Utils
    {   public static Texture2D ErrorTexture { get; internal set; }

        public static void LoadStringsFromFile(string absolutePath)
        {
            XmlSerializer xmlDeserializer;
            try
            {
                xmlDeserializer = new XmlSerializer(typeof(StringFile));
            }
            catch (Exception e)
            {
                Debug.Log("Unable to create a deserializer for type \"" + typeof(StringFile).Name + "\"");
                LogException(e);
                return;
            }

            NameTable nameTable = new NameTable();

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("", "");

            XmlParserContext parserContext =
                new XmlParserContext(nameTable, namespaceManager, "", XmlSpace.Default);

            XmlReaderSettings readerSettings = new XmlReaderSettings
            {
                NameTable = nameTable,
                ValidationFlags = XmlSchemaValidationFlags.None
            };

            try
            {
                using (XmlReader reader = XmlReader.Create(absolutePath, readerSettings, parserContext))
                {
                    StringFile strings;

                    try
                    {
                        strings = xmlDeserializer.Deserialize(reader) as StringFile;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Unable to deserialize file \"" + absolutePath + "\"");
                        LogException(e);
                        return;
                    }

                    if (strings.Strings == null)
                    {
                        Debug.Log(absolutePath + " is not recognized as a valid strings file. Please check your syntax.");
                        return;
                    }

                    foreach (StringFile.XmlString loadedString in strings.Strings)
                    {
                        StringList.mStrings.Add(loadedString.Key, loadedString.Value);

                        if (loadedString.Key.Contains("loading_hint"))
                        {
                            StringList.mLoadingHints.Add(loadedString.Value);
                        }
                    }

                    StringList.mLoadedFiles.Add(absolutePath);

                    Debug.Log("Successfully loaded " + strings.Strings.Length + " string(s) from " + absolutePath);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception thrown while attempting to create a stream reader for " + absolutePath + ". Exception thrown: ");
                Debug.Log(e);
                return;
            }

            StringList.loadFile(absolutePath, StringList.mStrings, false);
        }

        public static Texture2D LoadPngFromFile(string absolutePath)
        {
            Texture2D tex;

            if (File.Exists(absolutePath))
            {
                byte[] fileData = File.ReadAllBytes(absolutePath);
                tex = new Texture2D(2, 2);  //TODO fix this to be of arbitrary size
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                tex.name = Path.GetFileName(absolutePath);
            }
            else
            {
                Debug.Log("Error loading texture: \"" + absolutePath + "\"");
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

        public static T FindObjectByFilename<T>(this List<T> list, string filename) where T : UnityEngine.Object
        {
            try
            {
                return list.Find(x => x.name == filename);
            }
            catch (Exception e)
            {
                Debug.Log("Error loading file: \"" + filename + "\" with type: \"" + typeof(T));
                Debug.Log("Stacktrace: ");
                Debug.Log(e.ToString());

                return null;
            }
        }

        public static T FindObjectByFilepath<T>(this List<T> list, string filepath) where T : UnityEngine.Object
        {
            return FindObjectByFilename(list, Path.GetFileName(filepath));
        }

        public static bool IsValidTag(this string toCheck)
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

        public static string[] ListEmbeddedFiles()
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            return assembly.GetManifestResourceNames();
        }

        public static string GetFileNameFromAssemblyResourceName(string embeddedResourceName)
        {
            string[] split = embeddedResourceName.Split('.');

            return split[split.Length - 2] + '.' + split[split.Length - 1];
        }

        public static string[] GetFileNamesFromAssemblyResourceNames(string[] embeddedResourceNames)
        {
            string[] fileNames = new string[embeddedResourceNames.Length];

            for (var i = 0; i < embeddedResourceNames.Length; i++)
            {
                fileNames[i] = GetFileNameFromAssemblyResourceName(embeddedResourceNames[i]);
            }

            return fileNames;
        }

        public static Stream LoadEmbeddedFile(string filePath)
        {
            try
            {
                Assembly assembly = Assembly.GetCallingAssembly();

                return assembly.GetManifestResourceStream(filePath);
            } catch(Exception e)
            {
                Debug.Log("Error loading resource \"" + filePath + "\" from stream");
                LogException(e);
                return null;
            }
        }

        public static void LogException(Exception e, int tabCount = 0)
        {
            Debug.Log("Exception thrown:".PadLeft(4 * tabCount));
            Debug.Log(e.ToString().PadLeft(4 * tabCount));
            if (e.InnerException != null)
            {
                Debug.Log("Inner exception: ".PadLeft(4 * tabCount));
                LogException(e.InnerException, tabCount + 1);
            }
        }

        public static void KeyValuePairToDictionary<TK, TV>(this Dictionary<TK, TV> dictionary, KeyValuePair<TK, TV> kvp)
        {
            dictionary.Add(kvp.Key, kvp.Value);
        }

        public static void AddCollision(this GameObject gameObject)
        {
            if (gameObject.GetComponent<MeshFilter>() != null)
            {
                gameObject.AddComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            }
        }

        public static Texture2D FindTextureWithName(this List<Texture2D> textures, string name)
        {
            foreach (Texture2D texture in textures)
            {
                if (texture.name.Equals(name))
                {
                    return texture;
                }
            }

            Debug.Log("Couldn't find texture with filename \"" + name + "\"");

            return ErrorTexture;
        }

        public static List<Type> GetTypeByName(string className)
        {
            List<Type> matchingTypes = new List<Type>();
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (className.IndexOf('.') != -1) //Presence of a '.' indicates that the classname includes the namespace
                    {
                        if (type.FullName.Equals(className))
                        {
                            matchingTypes.Add(type);
                        }
                    }
                    else
                    {
                        if (type.Name.Equals(className))
                        {
                            matchingTypes.Add(type);
                        }
                    }
                }
            }

            return matchingTypes;
        }

        public static int ToInt(this bool toConvert)
        {
            return toConvert ? 1 : 0;
        }

        //Credit goes to https://stackoverflow.com/a/3446112/4352225
        public static string GetObjectPropertyValues(object @object)
        {
            string logString = "Object properties: ";

            foreach(PropertyInfo property in @object.GetType().GetProperties())
            {
                logString += "\r\n" + property.Name + ": " + (property.GetValue(@object, null) ?? "NULL");
            }

            return logString;
        }

        public static string GetObjectPropertyValues(GameObject gameObject)
        {
            string logString = "GameObject properties: ";

            logString += GetObjectPropertyValues((object) gameObject);

            logString += "\r\nComponents: ";

            foreach (Component component in gameObject.GetComponents(typeof(Component)))
            {
                logString += "\r\n" + component.GetType().Name + ": ";
                logString += GetObjectPropertyValues(component);
            }

            foreach (Transform subTransform in gameObject.transform)
            {
                logString += "\r\nSub-object properties: ";
                logString += GetObjectPropertyValues((object) subTransform.gameObject);

                logString += "\r\nComponents: ";

                foreach (Component component in subTransform.gameObject.GetComponents(typeof(Component)))
                {
                    logString += "\r\n" + component.GetType().Name + ": ";
                    logString += GetObjectPropertyValues(component);
                }
            }

            return logString;
        }

        public static void LogObjectProperties(object @object)
        {
            Debug.Log(GetObjectPropertyValues(@object));
        }

        public static void LogObjectProperties(GameObject gameObject)
        {
            Debug.Log(GetObjectPropertyValues(gameObject));
        }

        public static void RecursivelyAddColliders(this Transform t)
        {
            foreach (Transform transform in t)
            {
                if (transform.gameObject.GetComponent<MeshFilter>() != null)
                {
                    transform.gameObject.AddComponent<MeshCollider>().sharedMesh = transform.gameObject.GetComponent<MeshFilter>().sharedMesh;
                }

                if (t.childCount > 0)
                {
                    foreach (Transform subTransform in t)
                    {
                        subTransform.RecursivelyAddColliders();
                    }
                }
            }
        }
    }
}