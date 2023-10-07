using System.Collections.Generic;
using System.Linq;
using Harmony;
using Planetbase;

namespace PlanetbaseFramework.Patches.Planetbase.GameStateCredits
{
    [HarmonyPatch(typeof(global::Planetbase.GameStateCredits))]
    [HarmonyPatch(MethodType.Constructor)]
    public class ConstructorPatch
    {
        public const string Separator = "\n\n";

        public static Dictionary<ModBase, string> Credits { get; } = new Dictionary<ModBase, string>();

        // This patch adds credits for mods to the title "credits" screen.
        public static void Postfix(global::Planetbase.GameStateCredits __instance)
        {
            if (Credits.Values.Count == 0)
                return;

            // Add a break if not listed
            if (!__instance.mCreditsString.EndsWith(Separator))
                __instance.mCreditsString += Separator;

            foreach (var creditString in Credits.Values.Where(creditString => !string.IsNullOrEmpty(creditString.Trim())))
                __instance.mCreditsString += FormatCreditString(creditString) + Separator;

            __instance.mCreditsString = __instance.mCreditsString.Trim();
        }

        /// <summary>
        /// Uses the new Planetbase.GameStateCredits() logic to format the provided string.
        /// </summary>
        public static string FormatCreditString(string credits)
        {
            return credits
                .Replace("\n", Separator)
                .Replace("(h)", "<size=" + Singleton<GuiStyles>.getInstance().scaleDimension(26) + ">")
                .Replace("(/h)", "</size>");
        }
    }
}