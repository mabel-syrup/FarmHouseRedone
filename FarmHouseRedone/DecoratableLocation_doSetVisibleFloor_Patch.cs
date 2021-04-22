using Microsoft.Xna.Framework;
using StardewValley.Locations;
using xTile;

namespace FarmHouseRedone
{
    internal class DecoratableLocation_doSetVisibleFloor_Patch
    {
        private static void setMapTileIndexesInSquare(Map map, Rectangle floor, DecoratableLocation instance, int x,
            int y, int index)
        {
            if (floor.Contains(x, y))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y, index, "Back",
                    MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x + 1, y))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x + 1, y, index + 1, "Back",
                    MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x, y + 1))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y + 1, index + 16, "Back",
                    MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            if (floor.Contains(x + 1, y + 1))
                MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x + 1, y + 1, index + 17, "Back",
                    MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
        }

        internal static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            MapUtilities.MapMerger.DoSetVisibleFloor(whichRoom, which, __instance);
            //__instance.updateMap();

            ////Gather a list of all the floors in this map
            //List<Rectangle> floors = __instance.getFloors();
            //int index = 336 + which % 8 * 2 + which / 8 * 32;

            ////Report the index of the floor being pasted.
            //Logger.Log("Chosen index " + index + ".");

            ////Get the map for this DecoratableLocation
            //Map map = __instance.map;


            //Logger.Log("Applying floor rectangle...");

            ////It's possible that the number of saved floors is greater than the number of floors, so we'll just skip any after we reach the end.
            //if (floors.Count <= whichRoom)
            //{
            //    Logger.Log("Floor rectangle exceeded floors count!  You can ignore this if the farmhouse just upgraded, or you installed a new farmhouse mod.", StardewModdingAPI.LogLevel.Warn);
            //    return false;
            //}

            ////Find the region to paste in
            //Rectangle rectangle = floors[whichRoom];
            //int x = rectangle.X;
            //while (x < rectangle.Right)
            //{
            //    int y = rectangle.Y;
            //    while (y < rectangle.Bottom)
            //    {
            //        setMapTileIndexesInSquare(map, rectangle, __instance, x, y, index);
            //        y += 2;
            //    }
            //    x += 2;
            //}
            return false;
        }
    }
}