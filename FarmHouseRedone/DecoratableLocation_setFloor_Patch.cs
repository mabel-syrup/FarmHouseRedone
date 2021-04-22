using StardewValley.Locations;

namespace FarmHouseRedone
{
    internal class DecoratableLocation_setFloor_Patch
    {
        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            //if (!(__instance is FarmHouse))
            //    return true;
            if (!persist)
                return true;

            var floors = __instance.getFloors();

            if (__instance.floor.Count < floors.Count) MapUtilities.FacadeHelper.setMissingFloorsToDefault(__instance);

            //__instance.floor.SetCountAtLeast(floors.Count);
            if (whichRoom == -1)
            {
                for (var index = 0; index < __instance.floor.Count; ++index)
                    __instance.floor[index] = which;
            }
            else
            {
                if (whichRoom > __instance.floor.Count - 1 || whichRoom >= floors.Count)
                    return false;
                //FarmHouseState state = FarmHouseStates.getState(__instance as FarmHouse);

                var state = OtherLocations.DecoratableStates.getState(__instance);

                if (state.floorDictionary.ContainsKey(floors[whichRoom]))
                {
                    var roomLabel = state.floorDictionary[floors[whichRoom]];
                    Logger.Log("Finding all floors for room '" + roomLabel + "'...");
                    foreach (var floorData in state.floorDictionary)
                        if (floors.Contains(floorData.Key) && floorData.Value == roomLabel)
                        {
                            Logger.Log(floors.IndexOf(floorData.Key) + " was a part of " + roomLabel);
                            __instance.floor[floors.IndexOf(floorData.Key)] = which;
                        }
                }
                else
                {
                    __instance.floor[whichRoom] = which;
                }
            }

            return false;
        }
    }
}