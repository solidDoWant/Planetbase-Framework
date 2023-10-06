using System;
using System.Linq;
using Planetbase;
using UnityEngine;

namespace PlanetbaseFramework.GameMechanics.Prefabs
{
    public class ResourceTypePrefabLoader : PrefabLoader
    {
        public ResourceTypePrefabLoader() : base("Resources")
        {
        }

        public GameObject LoadPrefab<T>() where T : ResourceType
        {
            Utils.ThrowIfNotBaseGameType<T>();

            return LoadPrefab(typeof(T).Name);
        }

        public GameObject LoadPrefabUnpacked(string name, string suffix = "")
        {
            // Parameter validation
            switch (name)
            {
                case "Meal":
                    if (new[] { "" }.All(acceptedSuffix => suffix != acceptedSuffix))
                        throw new InvalidUnpackedSuffixException(name, suffix);

                    break;
                case "AlcoholicDrink":
                    if (suffix != "")
                        throw new InvalidUnpackedSuffixException(name, suffix);

                    break;
                default:
                    throw new ArgumentException(
                        "the provided resource name \"{name}\" does not have an \"unpacked\" model");
                }

            return LoadPrefab($"Unpacked{name}{suffix}");
        }

        public GameObject LoadPrefabUnpacked<T>(string suffix = "") where T : ResourceType
        {
            Utils.ThrowIfNotBaseGameType<T>();

            return LoadPrefabUnpacked(typeof(T).Name, suffix);
        }
    }
}