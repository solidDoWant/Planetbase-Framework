using System;
using Harmony;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.ComponentType
{
    // This patch is required because some change in the latest Planetbase release (1.3.8) causes
    // `loadPrefab` overrides on instances of children of `Planetbase.ComponentType` (such as
    // `loadPrefab` on `BaseComponentType` instances) to not properly override their parent. As a result,
    // the new `loadPrefab` overrides will not be called, rather, their parent 
    // `Planetbase.ComponentType::loadPrefab` is called instead. This patch entirely bypasses the parent
    // function call for ComponentTypes defined in non-Planetbase namespaces (such as
    // `PlanetbaseFramework.BaseComponentType` and it's descendants).

    [HarmonyPatch(typeof(global::Planetbase.ComponentType))]
    [HarmonyPatch("loadPrefab")]
    [HarmonyPatch(new Type[]{})]
    public class LoadPrefabPatch
    {
        public static bool Prefix(global::Planetbase.ComponentType __instance, ref GameObject __result)
        {
            if (__instance.GetType().Namespace == typeof(global::Planetbase.ComponentType).Namespace)
                return true;

            __result = __instance.loadPrefab();
            return false;
        }
    }
}