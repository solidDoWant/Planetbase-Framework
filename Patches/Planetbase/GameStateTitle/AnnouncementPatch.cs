using Harmony;

namespace PlanetbaseFramework.Patches.Planetbase.GameStateTitle
{
    [HarmonyPatch(typeof(global::Planetbase.GameStateTitle))]
    [HarmonyPatch("shouldDrawAnnouncement")]
    public class AnnouncementPatch
    {
        public static bool DrawAnnouncement { get; set; } = false;

        public static bool Prefix(ref bool __result)
        {
            __result = DrawAnnouncement;

            return false;
        }
    }
}