using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseFramework {

    ///<summery>
    ///Loads all mods from the mods directory
    ///</summery>
    public interface IModLoader {

        void setModDirectory(string path);
        List<ModBase> loadAll();

    }

    /*
    public class OldModLoaderWrapper : ModLoader, IModLoader {

        public List<ModBase> loadAll() {
            List<ModBase> list = new List<ModBase>();
            ModLoader.LoadAll();
            return ModLoader.ModList;
        }

        public void setModDirectory(string path) {
            throw new NotSupportedException();
        }

    }*/
}
