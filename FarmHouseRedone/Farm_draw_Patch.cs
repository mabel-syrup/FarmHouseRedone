using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace FarmHouseRedone
{
    internal class Farm_draw_Patch
    {
        //public static void baseDraw(Farm location, SpriteBatch b)
        //{
        //    IReflectedMethod drawCharacters = FarmHouseStates.reflector.GetMethod(location, "drawCharacters");
        //    IReflectedMethod drawFarmers = FarmHouseStates.reflector.GetMethod(location, "drawFarmers");
        //    drawCharacters.Invoke(b);
        //    foreach (Projectile projectile in location.projectiles)
        //        projectile.draw(b);
        //    drawFarmers.Invoke(b);
        //    if (location.critters != null)
        //    {
        //        for (int index = 0; index < location.critters.Count; ++index)
        //            location.critters[index].draw(b);
        //    }
        //    location.drawDebris(b);
        //    if (!Game1.eventUp || location.currentEvent != null && location.currentEvent.showGroundObjects)
        //    {
        //        foreach (KeyValuePair<Vector2, StardewValley.Object> pair in location.objects.Pairs)
        //            pair.Value.draw(b, (int)pair.Key.X, (int)pair.Key.Y, 1f);
        //    }
        //    foreach (TemporaryAnimatedSprite temporarySprite in location.TemporarySprites)
        //        temporarySprite.draw(b, false, 0, 0, 1f);
        //    location.interiorDoors.Draw(b);
        //    if (location.largeTerrainFeatures != null)
        //    {
        //        foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
        //            largeTerrainFeature.draw(b);
        //    }
        //    if (location.fishSplashAnimation != null)
        //        location.fishSplashAnimation.draw(b, false, 0, 0, 1f);
        //    if (location.orePanAnimation == null)
        //        return;
        //    location.orePanAnimation.draw(b, false, 0, 0, 1f);
        //}


        //public static void buildingDraw(Farm location, SpriteBatch b)
        //{


        //    foreach (Building building in location.buildings)
        //    {
        //        building.draw(b);
        //    }
        //}

        public static Tuple<int, int> removeShippingBin(List<CodeInstruction> codes)
        {
            Logger.Log("Removing shipping bin drawing...");
            var start = -1;
            var end = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("shippingBinLid"))
                {
                    Logger.Log("Shipping bin start: " + (i - 1));
                    start = i - 1;
                }

                if (start > -1 && codes[i].opcode == OpCodes.Callvirt && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("draw"))
                {
                    Logger.Log("Shipping bin end: " + i);
                    end = i + 1;
                    break;
                }
            }

            Logger.Log("Start: " + start + "\nEnd: " + end);
            return new Tuple<int, int>(start, end);
        }

        public static Tuple<int, int> removeMailBox(List<CodeInstruction> codes)
        {
            Logger.Log("Removing mailbox drawing...");
            var start = -1;
            var end = -1;
            var drawCalls = 0;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("get_mailbox()"))
                {
                    Logger.Log("Mailbox start: " + i);
                    start = i;
                }

                if (start > -1 && codes[i].opcode == OpCodes.Callvirt && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("Draw("))
                {
                    drawCalls++;
                    Logger.Log("Found mailbox draw call: " + i);
                    if (drawCalls > 1)
                    {
                        Logger.Log("Mailbox end: " + i);
                        end = i + 1;
                        break;
                    }
                }
            }

            Logger.Log("Start: " + start + "\nEnd: " + end);
            return new Tuple<int, int>(start, end);
        }

        public static Tuple<int, int> removeShadows(List<CodeInstruction> codes)
        {
            Logger.Log("Removing building shadow drawing...");
            var start = -1;
            var end = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("leftShadow"))
                {
                    Logger.Log("Found left shadow token: " + i);
                    for (var j = 0; j < i; j++)
                        if (codes[j].opcode == OpCodes.Ldarg_1)
                            start = j;
                    Logger.Log("Building shadow start: " + start);
                }

                if (start > -1 && codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("rightShadow"))
                {
                    for (var j = i; j < codes.Count; j++)
                        if (codes[j].opcode == OpCodes.Callvirt && codes[j].operand != null &&
                            codes[j].operand.ToString().Contains("Draw("))
                        {
                            Logger.Log("Building shadow end: " + j);
                            Logger.Log("Building shadow end was:\n" + codes[j].operand);
                            end = j + 1;
                            break;
                        }

                    if (end > -1)
                        break;
                }
            }

            Logger.Log("Start: " + start + "\nEnd: " + end);
            for (var i = start; i < end; i++)
                if (codes[i] != null)
                    Logger.Log(codes[i].ToString());
            return new Tuple<int, int>(start, end);
        }

        public static void removeShadowsNew(ref List<CodeInstruction> codes)
        {
            Logger.Log("Removing building shadow drawing...");
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("leftShadow"))
                    codes[i] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FarmState), "remove"));
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("middleShadow"))
                    codes[i] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FarmState), "remove"));
                if (codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("rightShadow"))
                    codes[i] = new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FarmState), "remove"));
            }
        }

        public static List<CodeInstruction> makeShadowsCodes()
        {
            var outCodes = new List<CodeInstruction>();
            outCodes.Add(new CodeInstruction(OpCodes.Ldarg_1));
            outCodes.Add(new CodeInstruction(OpCodes.Call,
                AccessTools.Method(typeof(FarmState), "draw", new Type[] {typeof(SpriteBatch)})));

            return outCodes;
        }

        public static void changeNote(ref List<CodeInstruction> codes)
        {
            var foundHasSeen = false;
            var adjustedSpot = false;
            var patchSummary = "Patched note draw code: \n";
            for (var i = 0; i < codes.Count; i++)
            {
                if (!foundHasSeen && codes[i].opcode == OpCodes.Ldfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("hasSeenGrandpaNote")) foundHasSeen = true;
                if (foundHasSeen && !adjustedSpot && codes[i].opcode == OpCodes.Ldsfld && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("mouseCursors"))
                {
                    var x = Convert.ToSingle(codes[i + 2].operand.ToString()) - 512f + FarmState.shrineLocation.X;
                    var y = Convert.ToSingle(codes[i + 3].operand.ToString()) - 448f + FarmState.shrineLocation.Y;
                    codes[i + 2] = new CodeInstruction(OpCodes.Ldc_R4, x);
                    codes[i + 3] = new CodeInstruction(OpCodes.Ldc_R4, y);
                    patchSummary += "Position now (" + x + ", " + y + ")\n";
                    adjustedSpot = true;
                }

                if (adjustedSpot && codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand != null &&
                    codes[i].operand.ToString().Contains("0.04"))
                {
                    var depth = (FarmState.shrineLocation.Y + 64f) / 64000f;
                    patchSummary += "Depth now " + depth;
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, depth);
                }
            }

            Logger.Log(patchSummary);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var startIndex = -1;
            var endIndex = -1;

            var codes = new List<CodeInstruction>(instructions);
            Logger.Log("Codes count: " + codes.Count);


            var mailbox = removeMailBox(codes);
            startIndex = mailbox.Item1;
            endIndex = mailbox.Item2;
            if (startIndex > -1 && endIndex > -1) codes.RemoveRange(startIndex, endIndex - startIndex);

            if (startIndex > -1)
                codes.InsertRange(startIndex, makeShadowsCodes());
            else
                Logger.Log(
                    "Mailbox draw code seems to have been altered by another mod.  This will result in unexpected behavior.",
                    LogLevel.Warn);

            removeShadowsNew(ref codes);

            changeNote(ref codes);

            //Logger.Log("Codes count after mailbox removal: " + codes.Count);

            //Tuple<int, int> buildingShadow = removeShadows(codes);
            //startIndex = buildingShadow.Item1;
            //endIndex = buildingShadow.Item2;
            //if (startIndex > -1 && endIndex > -1)
            //{
            //    for(int i = startIndex; i < endIndex; i++)
            //    {
            //        codes[i].labels.Clear();
            //    }
            //    codes.RemoveRange(startIndex, endIndex - startIndex);
            //}

            //Logger.Log("Codes count after shadows removal: " + codes.Count);

            //foreach (CodeInstruction codeInst in codes)
            //{
            //    if(codeInst != null)
            //        Logger.Log(codeInst.ToString());
            //}

            return codes.AsEnumerable();
        }

        public static void Postfix(SpriteBatch b, Farm __instance)
        {
            //FarmState.draw(b);
        }
    }
}