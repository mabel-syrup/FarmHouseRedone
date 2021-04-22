using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection.Emit;
using StardewModdingAPI;

namespace FarmHouseRedone
{
    internal class NPC_marriageDuties_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var indicesToDelete = new List<int>();

            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Contains("getPorchStandingSpot"))
                {
                    Logger.Log("Replacing vanilla getPorchStandingSpot() call at index " + i + "...");
                    codes[i] = new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(FarmState), nameof(FarmState.getPorchStandingSpotAndLog)));
                    Logger.Log("Index " + i + ": " + codes[i].ToString());
                    //codes[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Logger), nameof(Logger.Log), new Type[] { typeof(string) }));
                    indicesToDelete.Add(i - 1);
                }

            indicesToDelete.Reverse();

            foreach (var index in indicesToDelete)
            {
                //Logger.Log("Deleting index " + index + ": " + codes[index].ToString());
                //codes.RemoveAt(index);
                codes[index] = new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(FarmState), nameof(FarmState.setUpBaseFarm)));
                codes.Insert(index,
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), nameof(FarmState.init))));

                for (var readIndex = index - 3; readIndex < index + 4; readIndex++)
                    Logger.Log("Index " + readIndex + ": " + codes[readIndex].ToString());
                //codes.Insert(index, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), nameof(FarmState.getPorchStandingSpot))))
                //codes.Insert(index, new CodeInstruction(OpCodes.Ldstr, "Using patched getPorchStandingSpot()..."));
            }

            return codes.AsEnumerable();
        }
    }
}