using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone
{
    internal class FarmHouse_getFloors_Patch
    {
        public static void Postfix(ref List<Rectangle> __result, FarmHouse __instance)
        {
            __result.Clear();

            var state = OtherLocations.DecoratableStates.getState(__instance);

            __result = state.getFloors();

            if (__result.Count > 0)
            {
                return;
            }
            else
            {
                switch (__instance.upgradeLevel)
                {
                    case 0:
                        __result.Add(new Rectangle(1, 3, 10, 9));
                        break;
                    case 1:
                        __result.Add(new Rectangle(1, 3, 6, 9));
                        __result.Add(new Rectangle(7, 3, 11, 9));
                        __result.Add(new Rectangle(18, 8, 2, 2));
                        __result.Add(new Rectangle(20, 3, 9, 8));
                        break;
                    case 2:
                    case 3:
                        __result.Add(new Rectangle(1, 3, 12, 6));
                        __result.Add(new Rectangle(15, 3, 13, 6));
                        __result.Add(new Rectangle(13, 5, 2, 2));
                        __result.Add(new Rectangle(0, 12, 10, 11));
                        __result.Add(new Rectangle(10, 12, 11, 9));
                        __result.Add(new Rectangle(21, 17, 2, 2));
                        __result.Add(new Rectangle(23, 12, 11, 11));
                        break;
                }

                Logger.Log("Found " + __result.Count + " floors.");
            }
        }
    }

    //class FarmHouse_getForbiddenPetWarpTiles_Patch
    //{
    //    public static void Postfix(List<Rectangle> __result, FarmHouse __instance)
    //    {
    //        __result.Clear();
    //        FarmHouseState state = FarmHouseStates.getState(__instance);
    //        state.wallDictionary.Clear();
    //        //Logger.Log("Getting walls...");
    //        if (state.WallsData == null)
    //            FarmHouseStates.updateFromMapPath(__instance, __instance.mapPath);
    //        if (state.WallsData != "")
    //        {
    //            string[] wallArray = state.WallsData.Split(' ');
    //            for (int index = 0; index < wallArray.Length; index += 5)
    //            {
    //                try
    //                {
    //                    Rectangle rectResult = new Rectangle(Convert.ToInt32(wallArray[index]), Convert.ToInt32(wallArray[index + 1]), Convert.ToInt32(wallArray[index + 3]), Convert.ToInt32(wallArray[index + 4]));
    //                    __result.Add(rectResult);
    //                    state.wallDictionary[rectResult] = wallArray[index + 2];
    //                }
    //                catch (IndexOutOfRangeException)
    //                {
    //                    Logger.Log("Partial wall rectangle definition detected! (" + state.WallsData.Substring(index) + ")  Wall rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all wall definitions have exactly these 5 values.", LogLevel.Error);
    //                }
    //                catch (FormatException)
    //                {
    //                    string errorLocation = "";
    //                    for (int errorIndex = index; errorIndex < wallArray.Length && errorIndex - index < 5; errorIndex += 1)
    //                    {
    //                        errorLocation += wallArray[errorIndex] + " ";
    //                    }
    //                    Logger.Log("Incorrect wall rectangle format. (" + errorLocation + ")  Wall rectangles must be defined as\nX Y Identifier Width Height\nPlease ensure all wall definitions have exactly these 5 values.", LogLevel.Error);
    //                }
    //            }
    //            foreach (Rectangle floorRect in __result)
    //            {
    //                //Logger.Log("Found wall rectangle (" + floorRect.X + ", " + floorRect.Y + ", " + floorRect.Width + ", " + floorRect.Height + ")");
    //            }
    //        }
    //        else
    //        {
    //            switch (__instance.upgradeLevel)
    //            {
    //                case 0:
    //                    __result.Add(new Rectangle(1, 1, 10, 3));
    //                    state.wallDictionary[new Rectangle(1, 1, 10, 3)] = "House";
    //                    break;
    //                case 1:
    //                    __result.Add(new Rectangle(1, 1, 17, 3));
    //                    state.wallDictionary[new Rectangle(1, 1, 17, 3)] = "2";
    //                    __result.Add(new Rectangle(18, 6, 2, 2));
    //                    state.wallDictionary[new Rectangle(18, 6, 2, 2)] = "3";
    //                    __result.Add(new Rectangle(20, 1, 9, 3));
    //                    state.wallDictionary[new Rectangle(20, 1, 9, 3)] = "4";
    //                    break;
    //                case 2:
    //                case 3:
    //                    __result.Add(new Rectangle(1, 1, 12, 3));
    //                    __result.Add(new Rectangle(15, 1, 13, 3));
    //                    __result.Add(new Rectangle(13, 3, 2, 2));
    //                    __result.Add(new Rectangle(1, 10, 10, 3));
    //                    __result.Add(new Rectangle(13, 10, 8, 3));
    //                    __result.Add(new Rectangle(21, 15, 2, 2));
    //                    __result.Add(new Rectangle(23, 10, 11, 3));
    //                    break;
    //            }
    //        }
    //    }
    //}

    //public static void Prefix(xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, bool __result, FarmHouse __instance)
    //{
    //    if (__instance.map.GetLayer("Buildings").Tiles[tileLocation] != null)
    //    {

    //    }
    //}


    //Issue: should not make use of typeof(FarmHouse) ever
}