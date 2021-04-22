using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;

namespace FarmHouseRedone
{
    internal class Farm_addGrandpaCandles_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand.ToString().Contains("LooseSprites\\Cursors"))
                {
                    var offset = codes[i + 1].opcode == OpCodes.Ldloc_0 ? -1 : 0;
                    Logger.Log("Shrine anchor set to (" + FarmState.shrineLocation.X + ", " +
                               FarmState.shrineLocation.Y + ")");
                    codes[i + 6 + offset] = new CodeInstruction(OpCodes.Ldc_R4,
                        Convert.ToInt32(codes[i + 6 + offset].operand.ToString()) - 512f + FarmState.shrineLocation.X);
                    codes[i + 7 + offset] = new CodeInstruction(OpCodes.Ldc_R4,
                        Convert.ToInt32(codes[i + 7 + offset].operand.ToString()) - 448f + FarmState.shrineLocation.Y);
                    var patchInfo = "Patched candle sprite addition:\n";
                    for (var patchI = -2; patchI < 12 && i + patchI < codes.Count; patchI++)
                        patchInfo += codes[i + patchI].ToString() + "\n";
                    Logger.Log(patchInfo);
                }

            return codes;
        }
    }
}