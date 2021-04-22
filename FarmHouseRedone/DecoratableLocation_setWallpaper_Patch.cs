using StardewValley.Locations;

namespace FarmHouseRedone
{
    internal class DecoratableLocation_setWallpaper_Patch
    {
        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            if (!persist)
                return true;

            var walls = __instance.getWalls();

            Logger.Log("Checking wallpaper indexes before SetCountAtLeast...");
            for (var wallIndex = 0; wallIndex < __instance.wallPaper.Count; wallIndex++)
                Logger.Log("Wall " + wallIndex + " has a wallpaper index of " + __instance.wallPaper[wallIndex] + ".");

            Logger.Log(__instance.Name + " has " + walls.Count + " walls, and " + __instance.wallPaper.Count +
                       " wallpapers.");
            if (__instance.wallPaper.Count < walls.Count)
                MapUtilities.FacadeHelper.setMissingWallpaperToDefault(__instance);

            Logger.Log("Checking wallpaper indexes after SetCountAtLeast...");
            for (var wallIndex = 0; wallIndex < __instance.wallPaper.Count; wallIndex++)
                Logger.Log("Wall " + wallIndex + " has a wallpaper index of " + __instance.wallPaper[wallIndex] + ".");
            if (whichRoom == -1)
            {
                Logger.Log("Whichroom was -1, applying to all walls...");
                for (var index = 0; index < __instance.wallPaper.Count; ++index)
                    __instance.wallPaper[index] = which;
            }
            else
            {
                Logger.Log("Setting wallpaper to " + which + "...");
                if (whichRoom > __instance.wallPaper.Count - 1 || whichRoom >= walls.Count)
                    return false;
                //FarmHouseState state = FarmHouseStates.getState(__instance as FarmHouse);
                var state = OtherLocations.DecoratableStates.getState(__instance);

                if (state.wallDictionary.ContainsKey(walls[whichRoom]))
                {
                    var roomLabel = state.wallDictionary[walls[whichRoom]];
                    Logger.Log("Finding all walls for room '" + roomLabel + "'...");
                    foreach (var wallData in state.wallDictionary)
                        if (walls.Contains(wallData.Key) && wallData.Value == roomLabel)
                        {
                            Logger.Log(walls.IndexOf(wallData.Key) + " was a part of " + roomLabel);
                            __instance.wallPaper[walls.IndexOf(wallData.Key)] = which;
                        }
                }
                else
                {
                    __instance.wallPaper[whichRoom] = which;
                }
            }

            return false;
        }
    }
}