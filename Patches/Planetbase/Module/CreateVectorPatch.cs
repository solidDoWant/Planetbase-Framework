using Harmony;
using PlanetbaseFramework.GameMechanics.Buildings;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.Module
{
    [HarmonyPatch(typeof(global::Planetbase.Module))]
    [HarmonyPatch("create")]
    [HarmonyPatch(new []{typeof(Vector3), typeof(int), typeof(global::Planetbase.ModuleType)})]
    public class CreateVectorPatch
    {
        // This patch allows for the injection of custom Module instances, if the provided moduleType supports it.
        public static bool Prefix(Vector3 position, int sizeIndex, global::Planetbase.ModuleType moduleType)
        {
            // Don't run custom logic for base game moduletypes
            if (moduleType.GetType().AssemblyQualifiedName ==
                typeof(global::Planetbase.ModuleType).AssemblyQualifiedName)
                return true;

            // Don't run custom logic when the custom module type does not support it
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (!(moduleType is ICustomModuleProvider customModuleProvider)) 
                return true;

            customModuleProvider.Create(position, sizeIndex);

            // Don't run base game logic
            return false;
        }
    }
}