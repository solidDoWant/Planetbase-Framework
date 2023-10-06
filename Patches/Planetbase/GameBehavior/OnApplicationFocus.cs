using Harmony;
using UnityEngine;

namespace PlanetbaseFramework.Patches.Planetbase.GameBehavior
{
    [HarmonyPatch(typeof(global::Planetbase.GameBehaviour))]
    [HarmonyPatch("OnApplicationFocus")]
    public class OnApplicationFocus
    {
        // This patch adds a framerate cap equal to the targeted vsync refresh rate
        public static void Postfix(global::Planetbase.GameBehaviour __instance, bool focusStatus)
        {
            if (!focusStatus)
                return;

            if(QualitySettings.vSyncCount < 0)
                QualitySettings.vSyncCount = 0;

            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        }
    }
}