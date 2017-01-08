using Planetbase;
using System.IO;

namespace PlanetbaseFramework
{
    public abstract class ModBase
    {
        public ModBase()
        {
            StringList.loadStringsFromFolder(Util.getFilesFolder() + Path.DirectorySeparatorChar.ToString() + "Mods" + Path.DirectorySeparatorChar.ToString() + GetModName(), GetModName(), StringList.mStrings);
            UnityEngine.Debug.Log((object)"Loading " + GetModName());
        }

        public abstract void Init();

        public abstract void Update();

        public abstract string GetModName();
    }
}
