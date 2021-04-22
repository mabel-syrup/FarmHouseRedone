using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone
{
    internal class Farm_leftClick_Patch
    {
        public static Tuple<int, int> getPositionChecks(List<CodeInstruction> codes)
        {
            var start = -1;
            var end = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt && end != -1)
                {
                    Logger.Log("Ended the comparison section at " + i);
                    break;
                }

                if (codes[i].opcode == OpCodes.Blt || codes[i].opcode == OpCodes.Bgt)
                {
                    Logger.Log("Found a comparator...");
                    if (start == -1)
                    {
                        Logger.Log("Found beginning index at " + (i - 4));
                        start = i - 4;
                    }
                    else
                    {
                        Logger.Log("Found possible ending index at " + i);
                        end = i;
                    }
                }
                else
                {
                    Logger.Log("Index " + i + ": " + codes[i].opcode.Name + " " + codes[i].ToString());
                }
            }

            Logger.Log("Finished iterating through codes...");
            if (start == -1 || end == -1)
            {
                Logger.Log("Failed to find the position checks range!");
                return new Tuple<int, int>(-1, -1);
            }

            return new Tuple<int, int>(start, end - start);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Newobj)
                {
                    Logger.Log("Vector2 set to (" + (FarmState.shippingCrateLocation.X + 0.5f) + ", " +
                               FarmState.shippingCrateLocation.Y + ")");
                    codes[i - 2] = new CodeInstruction(OpCodes.Ldc_R4, FarmState.shippingCrateLocation.X + 0.5f);
                    codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, FarmState.shippingCrateLocation.Y);
                }

                if (codes[i].opcode == OpCodes.Blt)
                {
                    if (codes[i - 1].operand.ToString().Contains("71"))
                    {
                        Logger.Log("< 71 -> < " + FarmState.shippingCrateLocation.X + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int) FarmState.shippingCrateLocation.X);
                    }
                    else if (codes[i - 1].operand.ToString().Contains("13"))
                    {
                        Logger.Log("< 13 -> < " + (FarmState.shippingCrateLocation.Y - 1) + " @ index" + (i - 1));
                        codes[i - 1] =
                            new CodeInstruction(OpCodes.Ldc_I4_S, (int) FarmState.shippingCrateLocation.Y - 1);
                    }
                }
                else if (codes[i].opcode == OpCodes.Bgt)
                {
                    if (codes[i - 1].operand.ToString().Contains("72"))
                    {
                        Logger.Log("> 72 -> > " + (FarmState.shippingCrateLocation.X + 1) + " @ index" + (i - 1));
                        codes[i - 1] =
                            new CodeInstruction(OpCodes.Ldc_I4_S, (int) FarmState.shippingCrateLocation.X + 1);
                    }
                    else if (codes[i - 1].operand.ToString().Contains("14"))
                    {
                        Logger.Log("> 14 -> > " + FarmState.shippingCrateLocation.Y + " @ index" + (i - 1));
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (int) FarmState.shippingCrateLocation.Y);
                    }
                }
            }

            return codes;
        }


        public static bool Prefix(int x, int y, Farmer who, ref bool __result, Farm __instance)
        {
            if (FarmState.shippingCrateLocation.X != 71f || FarmState.shippingCrateLocation.Y != 14f)
            {
                if (who.ActiveObject == null || x / 64 < FarmState.shippingCrateLocation.X ||
                    x / 64 > FarmState.shippingCrateLocation.X + 1 || y / 64 < FarmState.shippingCrateLocation.Y - 1 ||
                    y / 64 > FarmState.shippingCrateLocation.Y || !who.ActiveObject.canBeShipped() ||
                    (double) Vector2.Distance(who.getTileLocation(), FarmState.shippingCrateLocation) > 2.0)
                    return true;
                __instance.getShippingBin(who).Add((Item) who.ActiveObject);
                __instance.lastItemShipped = (Item) who.ActiveObject;
                who.showNotCarrying();
                __instance.showShipment(who.ActiveObject, true);
                who.ActiveObject = null;
                __result = true;
                return false;
            }

            return true;
        }
    }
}