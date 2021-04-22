using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using StardewValley;
using StardewValley.Menus;

namespace FarmHouseRedone
{
    internal class Farm_checkAction_Patch
    {
        public static void shipItem(Item i, Farmer who)
        {
            if (i == null)
                return;
            who.removeItemFromInventory(i);
            Game1.getFarm().getShippingBin(who).Add(i);
            if (i is StardewValley.Object)
                Game1.getFarm().showShipment(i as StardewValley.Object, false);
            Game1.getFarm().lastItemShipped = i;
            if (Game1.player.ActiveObject != null)
                return;
            Game1.player.showNotCarrying();
            Game1.player.Halt();
        }

        public static bool Prefix(xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport,
            Farmer who, ref bool __result, Farm __instance)
        {
            if (tileLocation.X >= FarmState.shippingCrateLocation.X &&
                tileLocation.X <= FarmState.shippingCrateLocation.X + 1 &&
                tileLocation.Y >= FarmState.shippingCrateLocation.Y - 1 &&
                tileLocation.Y <= FarmState.shippingCrateLocation.Y)
            {
                //IReflectedMethod shipItem = FarmHouseStates.reflector.GetMethod(__instance, "shipItem");
                var itemGrabMenu = new ItemGrabMenu((IList<Item>) null, true, false,
                    new InventoryMenu.highlightThisItem(StardewValley.Utility.highlightShippableObjects),
                    new ItemGrabMenu.behaviorOnItemSelect(shipItem), "", (ItemGrabMenu.behaviorOnItemSelect) null, true,
                    true, false, true, false, 0, (Item) null, -1, (object) __instance);
                itemGrabMenu.initializeUpperRightCloseButton();
                var num1 = 0;
                itemGrabMenu.setBackgroundTransparency(num1 != 0);
                var num2 = 1;
                itemGrabMenu.setDestroyItemOnClick(num2 != 0);
                itemGrabMenu.initializeShippingBin();
                Game1.activeClickableMenu = (IClickableMenu) itemGrabMenu;
                __instance.playSound("shwip");
                if (Game1.player.FacingDirection == 1)
                    Game1.player.Halt();
                Game1.player.showCarrying();
                __result = true;
                return false;
            }

            return true;
        }

        public static Tuple<int, int> getComparisonSection(List<CodeInstruction> codes)
        {
            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Ldfld && codes[i + 4].opcode == OpCodes.Ldfld &&
                    codes[i + 8].opcode == OpCodes.Ldfld && codes[i + 12].opcode == OpCodes.Ldfld)
                    return new Tuple<int, int>(i - 1, i + 15);
            Logger.Log("Failed to find comparison section within checkAction!");
            return new Tuple<int, int>(-1, -1);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var comparisonRange = getComparisonSection(codes);

            if (comparisonRange.Item1 == -1 || comparisonRange.Item2 == -1) Logger.Log("Could not patch checkAction!");

            for (var i = comparisonRange.Item1; i < comparisonRange.Item2; i++)
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

            return codes;
        }
    }
}