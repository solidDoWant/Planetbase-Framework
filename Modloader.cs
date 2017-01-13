using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PlanetbaseFramework
{
    public class Modloader
    {
        public static List<ModBase> modList = new List<ModBase>();
        public static void loadMods()
        {
            if (Directory.Exists(ModBase.BasePath))
            {
                string[] files = Directory.GetFiles(ModBase.BasePath, "*.dll");
                foreach (string file in files)
                {
                    Type[] types = Assembly.LoadFile(file).GetTypes();
                    foreach(Type type in types)
                    {
                        if (typeof(ModBase).IsAssignableFrom(type))
                        {
                            Debug.Log("Loading mod \"" + type.Name.ToString() + "\" from file \"" + file + "\"");
                            ModBase mod = null;
                            try
                            {
                                mod = Activator.CreateInstance(type) as ModBase;
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Error loading mod from file: " + file + " of type: " + type.Name.ToString() + ". Exception thrown:");
                                Debug.Log(e.ToString() + ": " + e.Message);
                            }

                            if (mod != null)
                            {
                                try
                                {
                                    mod.Init();
                                    modList.Add(mod);
                                    Debug.Log("Loaded mod \"" + type.Name.ToString() + "\"");
                                }
                                catch (Exception e)
                                {
                                    Debug.Log("Error initializing mod from file: " + file + " of type: " + type.Name.ToString() + ". Exception thrown:");
                                    Debug.Log(e.ToString() + ": " + e.Message);
                                }
                            }
                            else
                            {
                                Debug.Log("Failed to load mod \"" + type.Name.ToString() + "\" from file \"" + file + "\"");
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
        public static void updateMods()
        {
            foreach(ModBase mod in modList)
            {
                try
                {
                    mod.Update();
                }
                catch (Exception e)
                {
                    Debug.Log("Error updating mod " + mod.ModName +  ". Exception thrown:");
                    Debug.Log(e.ToString() + ": " + e.Message);
                }
            }
        }
    }
}
