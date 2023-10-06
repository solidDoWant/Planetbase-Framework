using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework.GameMechanics.Prefabs
{
    public abstract class PrefabLoader
    {
        public string PrefabSubpath { get; protected set; }
        protected PrefabLoader(string prefabSubpath)
        {
            PrefabSubpath = prefabSubpath;
        }

        public GameObject LoadPrefab(string name)
        {
            return LoadPrefabFromPath($"Prefabs/{PrefabSubpath}/Prefab{name}");
        }

        /// <summary>
        /// Loads a base game prefab from the given path, and returns it as a game object.
        /// </summary>
        public static GameObject LoadPrefabFromPath(string prefabPath)
        {
            return Object.Instantiate(ResourceUtil.loadPrefab(prefabPath));
        }
    }
}