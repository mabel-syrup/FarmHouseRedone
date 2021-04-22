using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace FarmHouseRedone
{
    internal class FarmHouse_getWalls_Patch
    {
        public static void Postfix(ref List<Rectangle> __result, FarmHouse __instance)
        {
            __result.Clear();

            var state = OtherLocations.DecoratableStates.getState(__instance);

            __result = state.getWalls();

            if (__result.Count > 0)
                return;
            else
                switch (__instance.upgradeLevel)
                {
                    case 0:
                        __result.Add(new Rectangle(1, 1, 10, 3));
                        state.wallDictionary[new Rectangle(1, 1, 10, 3)] = "House";
                        break;
                    case 1:
                        __result.Add(new Rectangle(1, 1, 17, 3));
                        state.wallDictionary[new Rectangle(1, 1, 17, 3)] = "2";
                        __result.Add(new Rectangle(18, 6, 2, 2));
                        state.wallDictionary[new Rectangle(18, 6, 2, 2)] = "3";
                        __result.Add(new Rectangle(20, 1, 9, 3));
                        state.wallDictionary[new Rectangle(20, 1, 9, 3)] = "4";
                        break;
                    case 2:
                    case 3:
                        __result.Add(new Rectangle(1, 1, 12, 3));
                        __result.Add(new Rectangle(15, 1, 13, 3));
                        __result.Add(new Rectangle(13, 3, 2, 2));
                        __result.Add(new Rectangle(1, 10, 10, 3));
                        __result.Add(new Rectangle(13, 10, 8, 3));
                        __result.Add(new Rectangle(21, 15, 2, 2));
                        __result.Add(new Rectangle(23, 10, 11, 3));
                        break;
                }
        }
    }
}