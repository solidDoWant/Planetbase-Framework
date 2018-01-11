using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PlanetbaseFramework
{
    /*
     * This is the core class behind loading all mods. The patcher injects calls to loadMods() and updateMods() into the native PB code, which then
     * calls the methods in this file. This allows for minimal changes to PB's native code, while still allowing it to be extended.
     */
    public class Modloader
    {
        public static List<ModBase> ModList = new List<ModBase>();
        public static void LoadMods()
        {
            Debug.Log("Loading mod \"Planetbase Framework\"");
            ModBase frameworkMod = new FrameworkMod();
            ModList.Add(frameworkMod);
            frameworkMod.Init();
            Debug.Log("Loaded mod \"Planetbase Framework\"");

            if (Directory.Exists(ModBase.BasePath))
            {
                string[] files = Directory.GetFiles(ModBase.BasePath, "*.dll");
                foreach (string file in files)
                {
                    Type[] types;
                    try
                    {
                        types = Assembly.LoadFile(file).GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        Utils.LogException(e);
                        foreach (Exception loaderException in e.LoaderExceptions)
                        {
                            Utils.LogException(loaderException);
                        }

                        Debug.Log("************************ Note to modders: If you're seeing this exception, you probably are using a a post .Net 2.0.5.0 function.\r\n" +
                                  "For convenience I've made it so you can use mods compiled after 2.0.5.0, however modern features are not available. ************************");

                        continue;
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                        continue;
                    }

                    foreach(Type type in types)
                    {
                        if (typeof(ModBase).IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic && !Attribute.IsDefined(type, typeof(ModLoaderIgnoreAttribute)))
                        {
                            Debug.Log("Loading mod \"" + type.Name + "\" from file \"" + file + "\"");
                            ModBase mod = null;
                            try
                            {
                                mod = Activator.CreateInstance(type) as ModBase;
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Error loading mod from file: " + file + " of type: " + type.Name + ". Exception thrown:");
                                Utils.LogException(e);
                            }

                            if (mod != null)
                            {
                                try
                                {
                                    mod.Init();
                                    ModList.Add(mod);
                                    Debug.Log("Loaded mod \"" + mod.ModName + "\"");
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Error initializing mod \"" + mod.ModName + "\" from file: " + file + " of type: " + type.Name);
                                    Utils.LogException(e);
                                }
                            }
                            else
                            {
                                Debug.Log("Failed to load mod \"" + type.Name + "\" from file \"" + file + "\"");
                            }
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(ModBase.BasePath);
            }
        }

        public static void UpdateMods()
        {
            foreach(ModBase mod in ModList)
            {
                try
                {
                    mod.Update();
                }
                catch (Exception e)
                {
                    Debug.Log("Error updating mod " + mod.ModName);
                    Utils.LogException(e);
                }
            }
        }

        public static List<ModBase> GetModByType(Type modType)
        {
            List<ModBase> results = new List<ModBase>();    //oh boy, sure wish I could use linq right about now

            foreach (ModBase mod in ModList)
            {
                if (mod.GetType().Compare(modType))
                {
                    results.Add(mod);
                }
            }

            return results;
        }
    }
}