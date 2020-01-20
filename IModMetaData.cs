using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace PlanetbaseFramework {


    public interface IModMetaData {

        string GetName();
        Version GetVersion();
        string GetRootPath();
        ModBase GetMod();
        List<Texture2D> GetTextures();
        List<GameObject> GetObjects();

        // TODO: dependencies

    }

    public class SelfModMetaData : IModMetaData {

        private ModBase mod;

        public SelfModMetaData(ModBase mod) {
            this.mod = mod;
        }

        public ModBase GetMod() {
            return mod;
        }

        public string GetName() {
            return mod.ModName;
        }

        public Version GetVersion() {
            return mod.ModVersion;
        }

        public string GetRootPath() {
            return mod.ModPath;
        }

        public List<Texture2D> GetTextures() {
            return mod.ModTextures;
        }

        public List<GameObject> GetObjects() {
            return mod.ModObjects;
        }


    }

    /*
    // TODO: implement xml based meta data
    public class XmlModMetaData {

    }
    */
}
