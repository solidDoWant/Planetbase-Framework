using Planetbase;
using System;
using System.IO;
using UnityEngine;

namespace PlanetbaseFramework
{
    public abstract class ModBase
    {
        public ModBase(string ModName)
        {
            this.ModName = ModName;
            try
            {
                StringList.loadStringsFromFolder(this.ModPath, this.ModName, StringList.mStrings);
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't load string file for the mod " + this.ModName + " because of exception: " + e.ToString() + ": " + e.Message);
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
    }
}
