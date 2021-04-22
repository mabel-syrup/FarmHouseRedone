using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;

namespace FarmHouseRedone
{
    internal class Farm_UpdateWhenCurrentLocation_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            //int getPorchIndex = -1;

            //for(int i = 0; i < codes.Count; i++)
            //{
            //    if(codes[i].opcode == OpCodes.Callvirt && codes[i].operand.ToString().Contains("getPorchStandingSpot()"))
            //    {
            //        Logger.Log("Found the getPorchStandingSpot() call!  Index " + i);
            //        if(codes[i-1].opcode == OpCodes.Ldloc_1)
            //        {
            //            Logger.Log("Found the ldloc.1!");
            //            getPorchIndex = i - 1;
            //            break;
            //        }
            //    }
            //}

            //if(getPorchIndex == -1)
            //{
            //    Logger.Log("Failed to locate getPorchStandingSpot()...");
            //    return codes;
            //}

            //codes.RemoveAt(getPorchIndex);
            //codes[getPorchIndex] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FarmState), "getPorchStandingSpot"));

            var hasActiveFireplace = findFarmHouseFirePlaceCall(codes);
            if (hasActiveFireplace != -1)
            {
                codes[hasActiveFireplace] = new CodeInstruction(OpCodes.Ldc_I4_0);
                codes.RemoveAt(hasActiveFireplace - 1);
            }

            injectChimneyUpdateCall(ref codes);

            return codes;
        }

        public static void injectChimneyUpdateCall(ref List<CodeInstruction> codes)
        {
            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_MasterPlayer()"))
                {
                    codes.Insert(i,
                        new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(FarmState), nameof(FarmState.updateChimneySmoke))));
                    return;
                }
        }

        public static int findFarmHouseFirePlaceCall(List<CodeInstruction> codes)
        {
            var foundMasterPlayerCall = false;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand.ToString().Contains("get_MasterPlayer()"))
                {
                    Logger.Log("Found the get_MasterPlayer() call!  Index " + i);
                    foundMasterPlayerCall = true;
                }

                if (foundMasterPlayerCall && codes[i].opcode == OpCodes.Callvirt &&
                    codes[i].operand.ToString().Contains("hasActiveFireplace")) return i;
            }

            Logger.Log("Failed to locate the hasActiveFireplace call for the master farmhouse!");
            return -1;
        }
    }
}