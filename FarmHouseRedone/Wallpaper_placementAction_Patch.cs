using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;

namespace FarmHouseRedone
{
    internal class Wallpaper_placementAction_Patch
    {
        internal static bool Prefix(GameLocation location, int x, int y, Farmer who, ref bool __result,
            Wallpaper __instance)
        {
            if (who == null)
                who = Game1.player;
            if (who.currentLocation is DecoratableLocation)
                return true;
            var host = OtherLocations.FakeDecor.FakeDecorHandler.getHost(who.currentLocation);
            if (host == null)
                return true;

            //We have our host now, so we will instead be using it on that host.

            var point = new Point(x / 64, y / 64);

            if (__instance.isFloor.Value)
            {
                var floors = host.getFloors();
                for (var whichRoom = 0; whichRoom < floors.Count; ++whichRoom)
                    if (floors[whichRoom].Contains(point))
                    {
                        host.setFloor(__instance.ParentSheetIndex, whichRoom, true);
                        host.setFloors();
                        location.playSound("coin", NetAudio.SoundContext.Default);
                        __result = true;
                        return false;
                    }
            }
            else
            {
                var walls = host.getWalls();
                for (var whichRoom = 0; whichRoom < walls.Count; ++whichRoom)
                    if (walls[whichRoom].Contains(point))
                    {
                        host.setWallpaper(__instance.ParentSheetIndex, whichRoom, true);
                        host.setWallpapers();
                        location.playSound("coin", NetAudio.SoundContext.Default);
                        __result = true;
                        return false;
                    }
            }

            return true;
        }
    }
}