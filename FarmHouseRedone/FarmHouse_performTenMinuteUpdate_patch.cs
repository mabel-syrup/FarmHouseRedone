using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;

namespace FarmHouseRedone
{
    internal class FarmHouse_performTenMinuteUpdate_patch
    {
        internal static bool Prefix(int timeOfDay, FarmHouse __instance)
        {
            if (__instance is Cabin)
                return true;
            if (Game1.timeOfDay == 2200 && Game1.IsMasterGame)
                foreach (var character in __instance.characters)
                    if (character.isMarried())
                    {
                        Logger.Log(character.Name + " is going to bed...");
                        character.controller = (PathFindController) null;
                        var bedPoint = FarmHouseStates.getBedSpot(__instance, true);
                        Logger.Log(character.Name + " is attempting to path to " + bedPoint.ToString());
                        character.controller = new PathFindController((Character) character, (GameLocation) __instance,
                            bedPoint, 0);
                        if (character.controller.pathToEndPoint == null || !__instance.isTileOnMap(
                            character.controller.pathToEndPoint.Last<Point>().X,
                            character.controller.pathToEndPoint.Last<Point>().Y))
                        {
                            Logger.Log(character.Name + " could not path to " + bedPoint.ToString() + "!");
                            character.controller = (PathFindController) null;
                        }

                        return false;
                    }

            return true;
        }
    }
}