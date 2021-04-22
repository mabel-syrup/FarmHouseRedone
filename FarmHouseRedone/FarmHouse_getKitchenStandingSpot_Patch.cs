using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace FarmHouseRedone
{
    internal class FarmHouse_getKitchenStandingSpot_Patch
    {
        public static bool Prefix(FarmHouse __instance, Point __result)
        {
            return true;
/*
            try
            {
                var state = FarmHouseStates.getState(__instance);
                Logger.Log("Getting kitchen standing spot...");
                if (state.kitchenData == null)
                    FarmHouseStates.updateFromMapPath(__instance, __instance.mapPath.Value);
                if (state.kitchenData != "")
                {
                    var kitchenPoint = state.kitchenData.Split(' ');
                    if (kitchenPoint.Length < 2)
                    {
                        Logger.Log("Kitchen standing spot was defined, but did not have at least two numerical values!  Given '" + state.spouseRoomData + "'.", LogLevel.Error);
                        return true;
                    }
                    try
                    {
                        __result = new Point(Convert.ToInt32(kitchenPoint[0]), Convert.ToInt32(kitchenPoint[1]));
                        Logger.Log("Kitchen standing spot has been set to " + __result.ToString());
                        return false;
                    }
                    catch (FormatException)
                    {
                        Logger.Log("Spouse room was defined, but its values do not seem to be numerical!  Given '" + state.spouseRoomData + "'.", LogLevel.Error);
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
*/
        }
    }
}