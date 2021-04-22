using StardewValley.Locations;

namespace FarmHouseRedone
{
    internal class FarmHouse_setMapForUpgradeLevel_patch
    {
        internal static bool Prefix(int level, FarmHouse __instance)
        {
            Logger.Log("Setting map upgrade level...");
            __instance.upgradeLevel = level;
            var currentlyDisplayedUpgradeLevel =
                FarmHouseStates.reflector.GetField<int>(__instance, "currentlyDisplayedUpgradeLevel");
            var displayingSpouseRoom = FarmHouseStates.reflector.GetField<bool>(__instance, "displayingSpouseRoom");
            currentlyDisplayedUpgradeLevel.SetValue(level);
            var flag = __instance.owner.isMarried() && __instance.owner.spouse != null;

            if (displayingSpouseRoom.GetValue() && !flag)
                displayingSpouseRoom.SetValue(false);
            __instance.updateMap();
            FarmHouseStates.updateFromMapPath(__instance, __instance.mapPath.Value);
            if (flag || __instance.upgradeLevel == 3)
                __instance.showSpouseRoom();
            __instance.loadObjects();
            Logger.Log("Updating and setting wall defaults...");

            var state = OtherLocations.DecoratableStates.getState(__instance);

            //FarmHouseState state = FarmHouseStates.getState(__instance);
            if (state.WallsData == null || state.FloorsData == null)
                state.updateFromMapPath(__instance.mapPath.Value);

            //__instance.wallPaper.SetCountAtLeast(__instance.getWalls().Count);
            //__instance.floor.SetCountAtLeast(__instance.getFloors().Count);

            //if (state.WallsData.Equals(""))
            //{
            //    Logger.Log("No walls data defined, using basegame wall setting method.");
            //    baseGameSetWallpaper(__instance);
            //}
            //else
            //{
            Logger.Log("Setting wall and floor defaults...");
            MapUtilities.FacadeHelper.setWallpaperDefaults(__instance);
            //}
            //Logger.Log("Updating and setting floor defaults...");
            //if (state.FloorsData.Equals(""))
            //{
            //    Logger.Log("No floors data defined, using basegame wall setting method.");
            //    baseGameSetFloors(__instance);
            //}
            //else
            //{
            //Logger.Log("Debug using base game floors...");
            //baseGameSetFloors(__instance);
            //}
            Logger.Log("Clearing light glows...");
            __instance.lightGlows.Clear();
            Logger.Log("Done upgrading the house.");
            for (var wallIndex = 0; wallIndex < __instance.wallPaper.Count; wallIndex++)
            {
                Logger.Log("Wall " + wallIndex + " has a wallpaper index of " + __instance.wallPaper[wallIndex] + ".");
                __instance.setWallpaper(__instance.wallPaper[wallIndex], wallIndex, true);
            }

            return false;
        }

        internal static void baseGameSetFloors(FarmHouse house)
        {
            if (house.floor.Count > 0)
            {
                if (house.upgradeLevel == 1 && house.floor.Count == 1)
                {
                    house.setFloor(house.floor[0], 1, true);
                    house.setFloor(house.floor[0], 2, true);
                    house.setFloor(house.floor[0], 3, true);
                    house.setFloor(22, 0, true);
                }

                if (house.upgradeLevel == 2 && house.floor.Count == 3)
                {
                    var which = house.floor[3];
                    house.setFloor(house.floor[2], 5, true);
                    house.setFloor(house.floor[0], 3, true);
                    house.setFloor(house.floor[1], 4, true);
                    house.setFloor(which, 6, true);
                    house.setFloor(1, 0, true);
                    house.setFloor(31, 1, true);
                    house.setFloor(31, 2, true);
                }
            }
        }

        internal static void baseGameSetWallpaper(FarmHouse house)
        {
            if (house.wallPaper.Count > 0)
            {
                if (house.upgradeLevel == 1 && house.floor.Count == 1)
                {
                    house.setFloor(house.floor[0], 1, true);
                    house.setFloor(house.floor[0], 2, true);
                    house.setFloor(house.floor[0], 3, true);
                    house.setFloor(22, 0, true);
                }

                if (house.upgradeLevel == 2 && house.wallPaper.Count <= 4)
                {
                    house.setWallpaper(house.wallPaper[0], 4, true);
                    house.setWallpaper(house.wallPaper[2], 6, true);
                    house.setWallpaper(house.wallPaper[1], 5, true);
                    house.setWallpaper(11, 0, true);
                    house.setWallpaper(61, 1, true);
                    house.setWallpaper(61, 2, true);
                }

                if (house.upgradeLevel == 2 && house.floor.Count == 3)
                {
                    var which = house.floor[3];
                    house.setFloor(house.floor[2], 5, true);
                    house.setFloor(house.floor[0], 3, true);
                    house.setFloor(house.floor[1], 4, true);
                    house.setFloor(which, 6, true);
                    house.setFloor(1, 0, true);
                    house.setFloor(31, 1, true);
                    house.setFloor(31, 2, true);
                }
            }
            else
            {
                Logger.Log("No walls present!");
            }
        }
    }
}