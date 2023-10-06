using Harmony;

namespace PlanetbaseFramework.Patches.Planetbase.GameStateSettings
{
    [HarmonyPatch(typeof(global::Planetbase.GameStateSettings))]
    [HarmonyPatch("isValidResolution")]
    public class IsValidResolutionPatch
    {
        // This patch removes the aspect ratio check, which allows players
        // to use ultrawide and portrait monitors without stretching.
        public static bool Prefix(int width, int height, out bool __result)
        {
            __result = width >= 640 && height >= 480;

            return false;
        }
    }
}