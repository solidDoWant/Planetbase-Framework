using Harmony;

namespace PlanetbaseFramework.Patches.Planetbase.AnnouncementManager
{
    [HarmonyPatch(typeof(global::Planetbase.AnnouncementManager))]
    [HarmonyPatch("downloadThread")]
    public class DownloadThreadPatch
    {
        public static bool AllowDownload { get; set; } = false;

        // This patch prevents the game from "phoning home" and downloading
        // announcements
        public static bool Prefix()
        {
            return AllowDownload;
        }
    }
}