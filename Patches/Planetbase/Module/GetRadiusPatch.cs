using Harmony;

namespace PlanetbaseFramework.Patches.Planetbase.Module
{
    [HarmonyPatch(typeof(global::Planetbase.Module))]
    [HarmonyPatch("getRadius")]
    public class GetRadiusPatch
    {
        // This patch allows for an arbitrarily high module size index.
        public static bool Prefix(global::Planetbase.Module __instance, out float __result)
        {
            __result = ReplacementLogic.GetValidSize(__instance.mSizeIndex) * 0.5f;

            // Skip the original function call
            return false;
        }
    }
}