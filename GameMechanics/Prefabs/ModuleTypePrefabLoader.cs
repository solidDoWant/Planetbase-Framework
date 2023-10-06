using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework.GameMechanics.Prefabs
{
    public class ModuleTypePrefabLoader : PrefabLoader
    {
        public ModuleTypePrefabLoader() : base("Modules")
        {
        }

        public GameObject LoadPrefab(string moduleTypeName, int sizeIndex)
        {
            return LoadPrefab($"Prefab{moduleTypeName}{sizeIndex + 1}");
        }

        public GameObject LoadPrefab<T>(int sizeIndex) where T : ModuleType
        {
            Utils.ThrowIfNotBaseGameType<T>();

            var moduleTypeName = typeof(T).Name.Replace(nameof(ModuleType), string.Empty);
            return LoadPrefab(moduleTypeName, sizeIndex);
        }
    }
}