using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Locations;
using xTile;

namespace FarmHouseRedone
{
    internal class FarmHouse_doSetVisibleFloor_Patch
    {
        private static void setMapTileIndexesInSquare(Map map, Rectangle floor, FarmHouse instance, int x, int y,
            int index)
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

        internal static bool Prefix(int whichRoom, int which, FarmHouse __instance)
        {
            __instance.updateMap();

            //Gather a list of all the floors in this map
            var floors = __instance.getFloors();
            var index = 336 + which % 8 * 2 + which / 8 * 32;

            //Report the index of the floor being pasted.
            Logger.Log("Chosen index " + index + ".");

            //Get the map for this DecoratableLocation
            var map = __instance.map;


            Logger.Log("Applying floor rectangle...");

            //It's possible that the number of saved floors is greater than the number of floors, so we'll just skip any after we reach the end.
            if (floors.Count <= whichRoom)
            {
                Logger.Log(
                    "Floor rectangle exceeded floors count!  You can ignore this if the farmhouse just upgraded, or you installed a new farmhouse mod.",
                    LogLevel.Warn);
                return false;
            }

            //Find the region to paste in
            var rectangle = floors[whichRoom];
            var x = rectangle.X;
            while (x < rectangle.Right)
            {
                var y = rectangle.Y;
                while (y < rectangle.Bottom)
                {
                    setMapTileIndexesInSquare(map, rectangle, __instance, x, y, index);
                    y += 2;
                }

                x += 2;
            }

            return false;
        }


        //static void setMapTileIndexIfOnFloorSheet(Map map, DecoratableLocation instance, int x, int y, int index, string layer, int tileSheet, int tileSheetToMatch)
        //{
        //    if (map.GetLayer(layer).Tiles[x, y] != null && map.GetLayer(layer).Tiles[x, y].TileSheet.Equals((object)map.TileSheets[tileSheetToMatch]) && map.GetLayer(layer).Tiles[x, y].TileIndex >= 336)
        //        instance.setMapTileIndex(x, y, index, layer, tileSheet);
        //}

        //public static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        //{
        //    if (!(__instance is FarmHouse))
        //        return true;
        //    List<Microsoft.Xna.Framework.Rectangle> floors = __instance.getFloors();
        //    int index = 336 + which % 8 * 2 + which / 8 * 32;
        //    Logger.Log("Chosen index " + index + ".");
        //    if (whichRoom == -1)
        //    {
        //        foreach (Microsoft.Xna.Framework.Rectangle rectangle in floors)
        //        {
        //            int x = rectangle.X;
        //            while (x < rectangle.Right)
        //            {
        //                int y = rectangle.Y;
        //                while (y < rectangle.Bottom)
        //                {
        //                    if (rectangle.Contains(x, y))
        //                        setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y, index, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                    if (rectangle.Contains(x + 1, y))
        //                        setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y, index + 1, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                    if (rectangle.Contains(x, y + 1))
        //                        setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y + 1, index + 16, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                    if (rectangle.Contains(x + 1, y + 1))
        //                        setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y + 1, index + 17, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                    y += 2;
        //                }
        //                x += 2;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (floors.Count <= whichRoom)
        //        {
        //            Logger.Log("Floor rectangle exceeded floors count!", StardewModdingAPI.LogLevel.Warn);
        //            return false;
        //        }
        //        //List<Rectangle> connnectedFloors = new List<Rectangle>();
        //        //connnectedFloors.Add(floors[whichRoom]);
        //        //if (FarmHouseStates.floorDictionary.ContainsKey(floors[whichRoom]))
        //        //{
        //        //    string roomString = FarmHouseStates.floorDictionary[floors[whichRoom]];
        //        //    foreach (KeyValuePair<Rectangle, string> floorDefinition in FarmHouseStates.floorDictionary)
        //        //    {
        //        //        if (floorDefinition.Value.Equals(roomString))
        //        //            connnectedFloors.Add(floorDefinition.Key);
        //        //    }
        //        //}
        //        //foreach (Rectangle rectangle in connnectedFloors)
        //        //{
        //        Rectangle rectangle = floors[whichRoom];
        //        int x = rectangle.X;
        //        while (x < rectangle.Right)
        //        {
        //            int y = rectangle.Y;
        //            while (y < rectangle.Bottom)
        //            {
        //                if (rectangle.Contains(x, y))
        //                    setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y, index, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                if (rectangle.Contains(x + 1, y))
        //                    setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y, index + 1, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                if (rectangle.Contains(x, y + 1))
        //                    setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x, y + 1, index + 16, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                if (rectangle.Contains(x + 1, y + 1))
        //                    setMapTileIndexIfOnFloorSheet(__instance.map, __instance, x + 1, y + 1, index + 17, "Back", 0, FarmHouseStates.wallAndFloorsSheet);
        //                y += 2;
        //            }
        //            x += 2;
        //        }
        //    }
        //    return false;
        //}
    }
}