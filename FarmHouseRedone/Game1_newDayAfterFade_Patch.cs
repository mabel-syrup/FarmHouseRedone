using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;

namespace FarmHouseRedone
{
    internal class Game1_newDayAfterFade_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var bedSpotCall = findBedSpotCall(codes);
            if (bedSpotCall == -1)
            {
                Logger.Log("Could not find getBedSpot() in Game1_newDayAfterFade!");
                return codes.AsEnumerable();
            }

            var bedSpotInstruction = new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(FarmHouseStates), nameof(FarmHouseStates.getMainBedSpot)));
            Logger.Log("Setting index " + bedSpotCall + ":\n" + codes[bedSpotCall].ToString() + "\nTo:\n" +
                       bedSpotInstruction.ToString());
            codes[bedSpotCall] = bedSpotInstruction;
            Logger.Log("Removing indices " + (bedSpotCall - 3) + " - " + (bedSpotCall - 1) + ":");
            for (var removedIndex = bedSpotCall - 3; removedIndex < bedSpotCall; removedIndex++)
                Logger.Log("Index " + removedIndex + ": " + codes[removedIndex].ToString());
            codes.RemoveRange(bedSpotCall - 3, 3);

            return codes.AsEnumerable();
        }

        public static int findBedSpotCall(List<CodeInstruction> codes)
        {
            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("getBedSpot()"))
                    return i;
                //codes[i] = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(FarmHouseStates), nameof(FarmHouseStates.getBedSpot)));
                ////codes[i] = new CodeInstruction(OpCodes.Callvirt, nameof(FarmHouseStates.getBedSpot));
                //Logger.Log("Patched bed location:\n" + codes[i].ToString());
            return -1;
        }
    }
}