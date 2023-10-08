using Harmony;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.ModuleType
{
    // This patch is required because some change in the latest Planetbase release (1.3.8) causes
    // `loadPrefab` overrides on instances of children of `Planetbase.ModuleType` (such as
    // `loadPrefab` on `BaseModuleType` instances) to not properly override their parent. As a result,
    // the new `loadPrefab` overrides will not be called, rather, their parent 
    // `Planetbase.ModuleType::loadPrefab` is called instead. This patch entirely bypasses the parent
    // function call for ModuleTypes defined in non-Planetbase namespaces (such as
    // `PlanetbaseFramework.BaseModuleType` and it's descendants).

    [HarmonyPatch(typeof(global::Planetbase.ModuleType))]
    [HarmonyPatch("loadPrefab")]
    public class LoadPrefabPatch
    {
        public static bool Prefix(global::Planetbase.ModuleType __instance, int sizeIndex, ref GameObject __result)
        {
            if (__instance.GetType().Namespace == typeof(global::Planetbase.ModuleType).Namespace)
                return true;

            __result = __instance.loadPrefab(sizeIndex);
            return false;
        }
    }
}