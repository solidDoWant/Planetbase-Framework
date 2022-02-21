using Harmony;
using System;
using System.IO;

namespace PlanetbaseFramework.Patches.Planetbase.Util
{
    [HarmonyPatch(typeof(global::Planetbase.Util))]
    [HarmonyPatch("getFilesFolder")]
    public class FilesFolderPatch
    {
        public static string FilesFolderPath { get; } =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Planetbase");

        /// <summary>
        /// Fixes a stupid problem with how PB generates the path to the user's folder.
        /// </summary>
        public static bool Prefix(ref string __result)
        {
            __result = FilesFolderPath;

            return false;
        }
    }
}
