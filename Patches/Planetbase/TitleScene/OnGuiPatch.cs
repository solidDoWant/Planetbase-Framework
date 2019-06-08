using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;
using Planetbase;

namespace PlanetbaseFramework.Patches.Planetbase.TitleScene
{
    [HarmonyPatch(typeof(global::Planetbase.TitleScene))]
    [HarmonyPatch("onGui")]
    public class OnGuiPatch
    {
        private static string FrameworkVersion { get; } = FrameworkMod.ModVersion.ToString();

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionArray = instructions.ToArray();
            foreach (var instruction in instructionArray)
            {
                if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string operand &&
                    operand == Definitions.VersionNumber)
                {
                    instruction.operand += $" [P {FrameworkVersion}]";
                    break;
                }
            }

            return instructionArray;
        }
    }
}