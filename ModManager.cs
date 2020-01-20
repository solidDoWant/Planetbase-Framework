using System;
using System.Collections.Generic;
using UnityEngine;
using Planetbase;

namespace PlanetbaseFramework
{



    public class ModManager
    {

        private static ModManager instance = null;

        public static ModManager getInstance()
        {
            if (instance == null)
            {
                instance = new ModManager();
            }
            return instance;
        }



        private Dictionary<string, IModMetaData> modMetaDatas = new Dictionary<string, IModMetaData>();

        private Dictionary<string, ModBase> mods = new Dictionary<string, ModBase>();
        private Dictionary<string, ModBase> activeMods = new Dictionary<string, ModBase>();

        private IModConfig modConfig;
        //private IModLoader modLoader;

        public ModManager()
        {
            init();
        }


        private void init()
        {
            modConfig = new SimpleFileBasedModConfig(ModBase.BasePath + "/" + FrameworkMod.DefaultModName + "/.mods.cfg");

            // register framework mod
            FrameworkMod mod = new FrameworkMod();
            RegisterMod(new SelfModMetaData(mod), mod);
            //leagcy support
            ModLoader.ModList.Add(mod);
        }

        /// <summary>
        /// Set the <see cref="IModConfig"/> implementation to use.
        /// </summary>
        /// <param name="modConfig">The mod config implementation.</param>
        public void setModConfig(IModConfig modConfig)
        {
            this.modConfig = modConfig;
        }


        /// <summary>
        /// Registering a mod to the ModManager.
        /// If the mod is newer
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="mod"></param>
        public bool RegisterMod(IModMetaData metaData, ModBase mod)
        {
            string name = metaData.GetName();
            if (modMetaDatas.ContainsKey(name))
            {
                /// TODO: Can't get the original version?! always 0.0.0.0 on both sides..
                Debug.Log($"{modMetaDatas[name].GetVersion()} <> {metaData.GetVersion()} = {modMetaDatas[name].GetVersion().CompareTo(metaData.GetVersion())}");
                if (modMetaDatas[name].GetVersion().CompareTo(metaData.GetVersion()) >= 0)
                {
                    Debug.Log($"Newer or equal version ({modMetaDatas[name].GetVersion()} <- {metaData.GetVersion()}) of mod {name} already exists , skipping registration.");
                    return false;
                }
                Debug.Log($"Older version of mod {name} already exists, removing..");
                if (isModActive(name))
                {
                    cleanupMod(mods[name]);
                    activeMods.Remove(name);
                }
                modMetaDatas.Remove(name);
                mods.Remove(name);
            }
            modMetaDatas[name] = metaData;
            mods[name] = mod;
            Debug.Log($"Mod {name} registered with version {metaData.GetVersion()}");
            if (mod.GetType() == typeof(FrameworkMod) || modConfig.isActive(name))
            {
                activeMods[name] = mod;
                initMod(mod);
                Debug.Log($"Mod {name} auto activated.");
            }
            return true;
        }

        private void initMod(ModBase mod)
        {
            try
            {
                mod.Init();
            }
            catch (Exception e)
            {
                Debug.Log($"Error initializing mod \"{mod.ModName}\"");
                Utils.LogException(e);
            }
        }

        private void cleanupMod(ModBase mod)
        {
            try
            {
                mod.Cleanup();
            }
            catch (Exception e)
            {
                Debug.Log($"Error cleanup mod \"{mod.ModName}\"");
                Utils.LogException(e);
            }
        }
        /// <summary>
        /// Update hook for the mods.
        /// TODO: Changing patcher, called from <see cref="ModLoader"/>, can be direct.
        /// </summary>
        // copied from ModLoader
        public void UpdateMods()
        {
            foreach (ModBase mod in activeMods.Values)
            {
                try
                {
                    mod.Update();
                }
                catch (Exception e)
                {
                    Debug.Log($"Error updating mod {mod.ModName}");
                    Utils.LogException(e);
                }
            }
        }


        public List<IModMetaData> GetMetaDatas()
        {
            return new List<IModMetaData>(modMetaDatas.Values);
        }

        public bool isModActive(string name)
        {
            return (activeMods.ContainsKey(name));
        }

        public void setModActive(string modName, bool state = true)
        {
            ModBase mod = mods[modName];
            if (state)
            {
                initMod(mod);
                activeMods.Add(modName, mod);
            }
            else
            {
                activeMods.Remove(modName);
                cleanupMod(mod);
            }
            modConfig.setActive(modName, state);
        }

    }


}
