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
    {
        /// <summary>
        /// The default texture to use when one fails to load
        /// </summary>
        public static Texture2D ErrorTexture { get; internal set; }

        /// <summary>
        /// Load the a XML file containing strings (for localization/translations)
        /// </summary>
        /// <param name="absolutePath">The absolute path to the XML file</param>
        public static void LoadStringsFromFile(string absolutePath)
        {
            //Setup the deserializer
            XmlSerializer xmlDeserializer;
            try
            {
                xmlDeserializer = new XmlSerializer(typeof(StringFile));
            }
            catch (Exception e)
            {
                Debug.Log($"Unable to create a deserializer for type \"{typeof(StringFile).Name}\"");
                LogException(e);
                return;
            }

            var nameTable = new NameTable();

            var namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("", "");

            var parserContext = new XmlParserContext(nameTable, namespaceManager, "", XmlSpace.Default);

            var readerSettings = new XmlReaderSettings
            {
                NameTable = nameTable,
                ValidationFlags = XmlSchemaValidationFlags.None
            };

            try
            {
                //Create the reader
                using (var reader = XmlReader.Create(absolutePath, readerSettings, parserContext))
                {
                    //Read and deserialize the file the file
                    StringFile deserializedStrings;
                    try
                    {
                        deserializedStrings = xmlDeserializer.Deserialize(reader) as StringFile;
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Unable to deserialize file \"{absolutePath}\"");
                        LogException(e);
                        return;
                    }

                    if (deserializedStrings?.Strings == null)
                    {
                        Debug.Log(
                            $"\"{absolutePath}\" is not recognized as a valid strings file. Please check your syntax."
                        );
                        return;
                    }

                    foreach (var loadedString in deserializedStrings.Strings)
                    {
                        //Add the strings to the list
                        StringList.mStrings.Add(loadedString.Key, loadedString.Value);

                        //Add loading hints
                        if (loadedString.Key.Contains("loading_hint"))
                        {
                            StringList.mLoadingHints.Add(loadedString.Value);
                        }
                    }

                    StringList.mLoadedFiles.Add(absolutePath);

                    Debug.Log(
                        $"Successfully loaded {deserializedStrings.Strings.Length} string(s) from \"{absolutePath}\""
                    );
                }
            }
            catch (Exception e)
            {
                Debug.Log(
                    $"Exception thrown while attempting to create a stream reader for \"{absolutePath}\". Exception thrown: "
                );
                Debug.Log(e);
                return;
            }

            StringList.loadFile(absolutePath, StringList.mStrings, false);
        }

        /// <summary>
        /// Load a PNG into a Texture2D object
        /// </summary>
        /// <param name="absolutePath">The absolute path to the PNG file</param>
        public static Texture2D LoadPngFromFile(string absolutePath)
        {
            Texture2D loadedTexture;

            if (File.Exists(absolutePath))
            {
                var fileData = File.ReadAllBytes(absolutePath);
                loadedTexture = new Texture2D(2, 2);  //TODO fix this to be of arbitrary size
                loadedTexture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                loadedTexture.name = Path.GetFileName(absolutePath);
            }
            else
            {
                Debug.Log($"Error loading texture: \"{absolutePath}\"");
                loadedTexture = ErrorTexture;
            }

            return loadedTexture;
        }

        /// <summary>
        /// Set the normal map on a texture. This should be called on any normal maps either at the init stage or the constructor of the mod.
        /// </summary>
        /// <param name="texture">The texture to update.</param>
        public static void SetNormalMap(this Texture2D texture)
        {
            var pixels = texture.GetPixels();

            for (var i = 0; i < pixels.Length; i++)
            {
                var temp = pixels[i];
                temp.r = pixels[i].g;
                temp.a = pixels[i].r;
                pixels[i] = temp;
            }

            texture.SetPixels(pixels);
        }

        public static T FindObjectByFilename<T>(this List<T> list, string filename) where T : UnityEngine.Object
        {
            try
            {
                return list.Find(x => x.name == filename);
            }
            catch (Exception e)
            {
                Debug.Log($"Error loading file: \"{filename}\" with type: \"typeof(T)\"");
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
            var assembly = Assembly.GetCallingAssembly();

            return assembly.GetManifestResourceNames();
        }

        public static string GetFileNameFromAssemblyResourceName(string embeddedResourceName)
        {
            var split = embeddedResourceName.Split('.');

            return split[split.Length - 2] + '.' + split[split.Length - 1];
        }

        public static string[] GetFileNamesFromAssemblyResourceNames(string[] embeddedResourceNames)
        {
            var fileNames = new string[embeddedResourceNames.Length];

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
                var assembly = Assembly.GetCallingAssembly();

                return assembly.GetManifestResourceStream(filePath);
            } catch(Exception e)
            {
                Debug.Log($"Error loading resource \"{filePath}\" from stream");
                LogException(e);
                return null;
            }
        }

        public static void LogException(Exception e, int tabCount = 0)
        {
            Debug.Log("Exception thrown:".PadLeft(4 * tabCount));
            Debug.Log(e.ToString().PadLeft(4 * tabCount));

            if (e.InnerException == null) return;

            Debug.Log("Inner exception: ".PadLeft(4 * tabCount));
            LogException(e.InnerException, tabCount + 1);
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
            foreach (var texture in textures)
            {
                if (texture.name.Equals(name))
                {
                    return texture;
                }
            }

            Debug.Log($"Couldn't find texture with filename \"{name}\"");

            return ErrorTexture;
        }

        public static List<Type> GetTypeByName(string className)
        {
            var matchingTypes = new List<Type>();
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
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
            var logString = "Object properties: ";

            foreach(var property in @object.GetType().GetProperties())
            {
                logString += $"\r\n{property.Name}: " + (property.GetValue(@object, null) ?? "NULL");
            }

            return logString;
        }

        public static string GetObjectPropertyValues(GameObject gameObject)
        {
            var logString = "GameObject properties: ";

            logString += GetObjectPropertyValues((object) gameObject);

            logString += "\r\nComponents: ";

            foreach (var component in gameObject.GetComponents(typeof(Component)))
            {
                logString += "\r\n" + component.GetType().Name + ": ";
                logString += GetObjectPropertyValues(component);
            }

            foreach (Transform subTransform in gameObject.transform)
            {
                logString += "\r\nSub-object properties: ";
                logString += GetObjectPropertyValues((object) subTransform.gameObject);

                logString += "\r\nComponents: ";

                foreach (var component in subTransform.gameObject.GetComponents(typeof(Component)))
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

        public static FrameworkMod GetFrameworkMod() =>
            ModLoader.ModList.Find(mod => mod.ModName.Equals("Planetbase Framework")) as FrameworkMod;

        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}