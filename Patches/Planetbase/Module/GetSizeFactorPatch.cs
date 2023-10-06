using Harmony;

namespace PlanetbaseFramework.Patches.Planetbase.Module
{
    [HarmonyPatch(typeof(global::Planetbase.Module))]
    [HarmonyPatch("getSizeFactor")]
    public class GetSizeFactorPatch
    {
        // This patch allows for an arbitrarily high module size index.
        public static bool Prefix(global::Planetbase.Module __instance, out float __result)
        {
            __result = 0.5f * ModuleType.ReplacementLogic.GetBaseCost(__instance.mSizeIndex) + 0.5f;

            // Skip the original function call
            return false;
        }
    }
}