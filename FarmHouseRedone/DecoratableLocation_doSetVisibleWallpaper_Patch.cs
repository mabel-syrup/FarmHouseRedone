using Microsoft.Xna.Framework;
using StardewValley.Locations;
using xTile;

namespace FarmHouseRedone
{
    internal class DecoratableLocation_doSetVisibleWallpaper_Patch
    {
        private static int getNewTileIndex(Map map, int x, int y, string layer, int destinationIndex)
        {
            if (map.GetLayer(layer).Tiles[x, y] == null)
                return -1;
            var currentIndex = map.GetLayer(layer).Tiles[x, y].TileIndex;
            var whichHeight = currentIndex % 48 / 16;
            return destinationIndex + whichHeight * 16;
        }

        private static void setMapTileIndexForAnyLayer(Map map, DecoratableLocation instance, int x, int y, int index)
        {
            MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y, getNewTileIndex(map, x, y, "Back", index),
                "Back", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y,
                getNewTileIndex(map, x, y, "Buildings", index), "Buildings",
                MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            MapUtilities.MapMerger.setMapTileIndexIfOnTileSheet(map, x, y, getNewTileIndex(map, x, y, "Front", index),
                "Front", MapUtilities.SheetHelper.getTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
        }

        internal static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            MapUtilities.MapMerger.DoSetVisibleWallpaper(whichRoom, which, __instance);
            ////if (!(__instance is FarmHouse))
            ////    return true;
            //__instance.updateMap();

            ////Gather a list of all the walls in this map
            //List<Rectangle> walls = __instance.getWalls();
            //int index = which % 16 + which / 16 * 48;

            ////Report the index of the wallpaper being pasted.
            //Logger.Log("Chosen index " + index + ".");

            ////Get the map for this DecoratableLocation
            //Map map = __instance.map;


            //Logger.Log("Applying wall rectangle...");

            ////It's possible that the number of saved wallpapers is greater than the number of walls, so we'll just skip any after we reach the end.
            //if (walls.Count <= whichRoom)
            //{
            //    Logger.Log("Wall rectangle exceeded walls count!  You can ignore this if the farmhouse just upgraded, or you installed a new farmhouse mod.", StardewModdingAPI.LogLevel.Warn);
            //    return false;
            //}

            ////Find the region to paste in
            //Rectangle rectangle = walls[whichRoom];
            //for (int x = rectangle.X; x < rectangle.Right; x++)
            //{
            //    for (int y = rectangle.Y; y < rectangle.Bottom; y++)
            //    {
            //        setMapTileIndexForAnyLayer(map, __instance, x, y, index);
            //    }
            //}
            return false;
        }
    }
}