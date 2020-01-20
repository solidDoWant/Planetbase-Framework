using Harmony;

namespace PlanetbaseFramework.Patches.Planetbase.GameManager
{
    [HarmonyPatch(typeof(global::Planetbase.GameManager))]
    [HarmonyPatch("update")]
    public class Update
    {
        public static void Postfix()
        {
            //ModLoader.UpdateMods();
            ModManager.getInstance().UpdateMods();
        }
    }
}