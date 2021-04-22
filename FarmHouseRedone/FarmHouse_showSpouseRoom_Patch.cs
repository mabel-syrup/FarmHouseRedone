using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace FarmHouseRedone
{
    internal class FarmHouse_showSpouseRoom_Patch
    {
        internal static bool Prefix(FarmHouse __instance)
        {
            var married = __instance.owner.isMarried() && __instance.owner.spouse != null;
            var displayingSpouseRoom = FarmHouseStates.reflector.GetField<bool>(__instance, "displayingSpouseRoom");
            var displayingSpouse = displayingSpouseRoom.GetValue() ? 1 : 0;
            displayingSpouseRoom.SetValue(married);
            __instance.updateMap();
            __instance.loadObjects();

            //Cellar stuff!
            if (__instance.upgradeLevel == 3)
            {
                pasteCellar(__instance);
                //__instance.warps.Add(new Warp(4, 25, "Cellar", 3, 2, false));
                //__instance.warps.Add(new Warp(5, 25, "Cellar", 4, 2, false));
                if (!Game1.player.craftingRecipes.ContainsKey("Cask"))
                    Game1.player.craftingRecipes.Add("Cask", 0);
            }

            if (!married)
                return false;
            __instance.loadSpouseRoom();
            return false;
        }

        internal static void pasteVanillaCellar(FarmHouse house)
        {
            house.setMapTileIndex(3, 22, 162, "Front", 0);
            house.removeTile(4, 22, "Front");
            house.removeTile(5, 22, "Front");
            house.setMapTileIndex(6, 22, 163, "Front", 0);
            house.setMapTileIndex(3, 23, 64, "Buildings", 0);
            house.setMapTileIndex(3, 24, 96, "Buildings", 0);
            house.setMapTileIndex(4, 24, 165, "Front", 0);
            house.setMapTileIndex(5, 24, 165, "Front", 0);
            house.removeTile(4, 23, "Back");
            house.removeTile(5, 23, "Back");
            house.setMapTileIndex(4, 23, 1043, "Back", 0);
            house.setMapTileIndex(5, 23, 1043, "Back", 0);
            house.setMapTileIndex(4, 24, 1075, "Back", 0);
            house.setMapTileIndex(5, 24, 1075, "Back", 0);
            house.setMapTileIndex(6, 23, 68, "Buildings", 0);
            house.setMapTileIndex(6, 24, 130, "Buildings", 0);
            house.setMapTileIndex(4, 25, 0, "Front", 0);
            house.setMapTileIndex(5, 25, 0, "Front", 0);
            house.removeTile(4, 23, "Buildings");
            house.removeTile(5, 23, "Buildings");
            house.warps.Add(new Warp(4, 25, "Cellar", 3, 2, false));
            house.warps.Add(new Warp(5, 25, "Cellar", 4, 2, false));
        }

        internal static void pasteCellar(FarmHouse house)
        {
            var state = FarmHouseStates.getState(house);
            if (state.levelThreeData == null)
                FarmHouseStates.updateFromMapPath(house, house.mapPath.Value);
            //Dictionary<Point, Tuple<Map, bool>> levelThreeUpgrades = new Dictionary<Point, Tuple<Map, bool>>();
            if (state.levelThreeData == "" && house.upgradeLevel == 3)
            {
                Logger.Log("Level three updgrades not defined, using vanilla...");
                //levelThreeUpgrades[new Point(3, 22)] = new Tuple<Map, bool> (FarmHouseStates.loadLevelThreeUpgradeIfPresent("Cellar"), false);
                var cellarMap = FarmHouseStates.loadLevelThreeUpgradeIfPresent("Cellar");
                if (cellarMap == null)
                {
                    Logger.Log("Couldn't paste the default cellar map, the file was not found!", LogLevel.Error);
                    return;
                }

                MapUtilities.MapMerger.pasteMap(house, cellarMap, 3, 22, MapUtilities.MapMerger.PASTE_PRESERVE_FLOORS);
                //pasteMapSection(house.map, cellarMap, new Point(3, 22), new Rectangle(0, 0, cellarMap.GetLayer("Back").LayerWidth, cellarMap.GetLayer("Back").LayerHeight), false);
                Logger.Log("Pasted the default cellar map at (3, 22).");
            }
            else
            {
                Logger.Log("Map defined level 3 upgrade data...");
                var levelThree = state.levelThreeData.Split(' ');
                var x = -1;
                var y = -1;
                var mapName = "Cellar";
                var destructive = false;
                var readerIndex = 0;
                //Read data of an undefined length, with optional parameters
                while (readerIndex < levelThree.Length)
                    //If this and the next item are both numbers, assume they are an X and Y coordinate.
                    if (isNumeric(levelThree[readerIndex]) && isNumeric(levelThree[readerIndex + 1]))
                    {
                        //We already found coordinates, so this must be the next definition
                        if (x != -1 && y != -1)
                        {
                            //Paste upgrade using data we have so far
                            var sectionMap = FarmHouseStates.loadLevelThreeUpgradeIfPresent(mapName);
                            if (sectionMap == null)
                            {
                                Logger.Log(
                                    "Failed to paste " + mapName + "_levelthree at (" + x + ", " + y +
                                    "), no map was found!", LogLevel.Error);
                            }
                            else
                            {
                                var sectionRectangle = new Rectangle(0, 0, sectionMap.GetLayer("Back").LayerWidth,
                                    sectionMap.GetLayer("Back").LayerHeight);
                                MapUtilities.MapMerger.pasteMap(house, sectionMap, x, y, destructive ? 0 : 2);
                                //pasteMapSection(house.map, sectionMap, new Point(x, y), sectionRectangle, destructive);
                            }

                            //Reset variables
                            x = -1;
                            y = -1;
                            mapName = "Cellar";
                            destructive = false;
                        }

                        //Set the coordinates
                        x = Convert.ToInt32(levelThree[readerIndex]);
                        y = Convert.ToInt32(levelThree[readerIndex + 1]);
                        //Skip a number, since we just looked at two at the same time
                        readerIndex += 2;
                    }
                    else if (x == -1 || y == -1)
                    {
                        Logger.Log(
                            "Improper level 3 upgrade data!  No coordinates appear to be present!  Please ensure all level 3 definitions begin with an X and Y coordinate.",
                            LogLevel.Error);
                        readerIndex++;
                    }
                    else
                    {
                        //Look for the -destructive flag
                        if (levelThree[readerIndex].StartsWith("-"))
                        {
                            var flag = levelThree[readerIndex].TrimStart('-').ToLower();
                            Logger.Log("Found flag: " + flag);
                            if (flag.StartsWith("d"))
                            {
                                Logger.Log("Interpreted as 'destructive' flag.");
                                destructive = true;
                            }
                        }
                        //If it wasn't numerical or prefixed by the '-' character, interpret it as the map name
                        else
                        {
                            mapName = levelThree[readerIndex];
                            Logger.Log("Map selected: " + mapName);
                        }

                        readerIndex++;
                    }

                if (x != -1 && y != -1)
                {
                    var sectionMap = FarmHouseStates.loadLevelThreeUpgradeIfPresent(mapName);
                    if (sectionMap == null)
                    {
                        Logger.Log(
                            "Failed to paste " + mapName + "_levelthree at (" + x + ", " + y + "), no map was found!",
                            LogLevel.Error);
                    }
                    else
                    {
                        var sectionRectangle = new Rectangle(0, 0, sectionMap.GetLayer("Back").LayerWidth,
                            sectionMap.GetLayer("Back").LayerHeight);
                        MapUtilities.MapMerger.pasteMap(house, sectionMap, x, y, destructive ? 0 : 2);
                        //pasteMapSection(house.map, sectionMap, new Point(x, y), sectionRectangle, destructive);
                    }
                }
            }
        }

        internal static bool isNumeric(string candidate)
        {
            int n;
            return int.TryParse(candidate, out n);
        }

        //internal static void pasteMapSection(Map houseMap, Map sectionMap, Point housePoint, Rectangle sectionRect, bool destructive = false)
        //{
        //    Logger.Log("Pasting map section named '" + sectionMap.Id + "' at " + housePoint.ToString() + ", with a bounds of " + sectionRect.ToString());
        //    //Iterate each coordinate within the pasted section
        //    for (int x = 0; x < sectionRect.Width; x++)
        //    {
        //        for (int y = 0; y < sectionRect.Height; y++)
        //        {
        //            pasteTile(houseMap, sectionMap, housePoint, x, y, "Back", destructive);
        //            pasteTile(houseMap, sectionMap, housePoint, x, y, "Buildings", destructive);
        //            pasteTile(houseMap, sectionMap, housePoint, x, y, "Front", destructive);
        //        }
        //    }
        //    foreach (KeyValuePair<string, PropertyValue> pair in sectionMap.Properties)
        //    {
        //        PropertyValue value = pair.Value;
        //        string propertyName = pair.Key;
        //        Logger.Log("Upgrade map contained Properties data, applying now...");
        //        if (propertyName.Equals("Warp"))
        //        {
        //            //propertyName = "Warp";
        //            Logger.Log("Adjusting warp property...");
        //            string[] warpParts = Utility.cleanup(value.ToString()).Split(' ');
        //            string warpShifted = "";
        //            for (int index = 0; index < warpParts.Length; index += 5)
        //            {
        //                try
        //                {
        //                    Logger.Log("Relative warp found: " + warpParts[index + 0] + " " + warpParts[index + 1] + " " + warpParts[index + 2] + " " + warpParts[index + 3] + " " + warpParts[index + 4]);
        //                    int xi = -1;
        //                    int yi = -1;
        //                    int xii = -1;
        //                    int yii = -1;
        //                    if (warpParts[0].StartsWith("~"))
        //                        xi = housePoint.X + Convert.ToInt32(warpParts[index + 0].TrimStart('~'));
        //                    else
        //                        xi = Convert.ToInt32(warpParts[0]);
        //                    if (warpParts[1].StartsWith("~"))
        //                        yi = housePoint.Y + Convert.ToInt32(warpParts[index + 1].TrimStart('~'));
        //                    else
        //                        yi = Convert.ToInt32(warpParts[1]);
        //                    if (warpParts[3].StartsWith("~"))
        //                        xii = housePoint.X + Convert.ToInt32(warpParts[index + 3].TrimStart('~'));
        //                    else
        //                        xii = Convert.ToInt32(warpParts[3]);
        //                    if (warpParts[4].StartsWith("~"))
        //                        yii = housePoint.Y + Convert.ToInt32(warpParts[index + 4].TrimStart('~'));
        //                    else
        //                        yii = Convert.ToInt32(warpParts[4]);
        //                    string returnWarp = xi + " " + yi + " " + warpParts[index + 2] + " " + xii + " " + yii + " ";
        //                    Logger.Log("Relative warp became " + returnWarp);
        //                    warpShifted += returnWarp;
        //                }
        //                catch (IndexOutOfRangeException)
        //                {
        //                    Logger.Log("Incomplete warp definition found!  Please ensure all warp definitions are formatted as 'X Y map X Y'", LogLevel.Warn);
        //                }
        //                catch (FormatException)
        //                {
        //                    Logger.Log("Invalid warp definition found!  Please ensure all warp definitions use numbers fro the X and Y coordinates!", LogLevel.Warn);
        //                }
        //            }
        //            value = warpShifted.Trim(' ');
        //            Logger.Log("Warp property is now " + value.ToString());
        //        }
        //        if (!houseMap.Properties.ContainsKey(propertyName))
        //        {
        //            Logger.Log("FarmHouse map did not have a '" + propertyName + "' property, setting it to '" + propertyName + "'...");
        //            houseMap.Properties.Add(propertyName, value);
        //        }
        //        else
        //        {
        //            PropertyValue houseValue = houseMap.Properties[propertyName];
        //            Logger.Log("Farmhouse map already had a '" + propertyName + "' value, appending...");
        //            houseMap.Properties[propertyName] = (houseValue.ToString() + " " + value.ToString()).Trim(' ');
        //            Logger.Log(propertyName + " is now " + houseMap.Properties[propertyName].ToString());
        //        }
        //    }
        //}

        //internal static void pasteTile(Map houseMap, Map sectionMap, Point housePoint, int x, int y, string layer, bool destructive = false)
        //{
        //    //If the pasted map has a tile on the Back layer at that location
        //    Logger.Log("Trying to paste index (" + x + ", " + y + ") local; (" + (x + housePoint.X) + ", " + (y + housePoint.Y) + ") global...");
        //    if (sectionMap.GetLayer(layer).Tiles[x, y] != null)
        //    {
        //        Logger.Log("Local tile was not null...");
        //        //Tile sectionTile = sectionMap.GetLayer("Buildings").Tiles[x, y];
        //        Tile sectionTile = sectionMap.GetLayer(layer).Tiles[x, y];
        //        TileSheet sheet = getEquivalentSheet(houseMap, sectionTile.TileSheet);
        //        Logger.Log("Setting global tile to local tile...");


        //        if (sectionTile is AnimatedTile)
        //        {
        //            int framesCount = (sectionTile as AnimatedTile).TileFrames.Length;
        //            StaticTile[] frames = new StaticTile[framesCount];
        //            for (int i = 0; i < framesCount; i++)
        //            {
        //                StaticTile frame = (sectionTile as AnimatedTile).TileFrames[i];
        //                frames[i] = new StaticTile(houseMap.GetLayer(layer), sheet, frame.BlendMode, frame.TileIndex);
        //            }
        //            houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] = new AnimatedTile(houseMap.GetLayer(layer), frames, (sectionTile as AnimatedTile).FrameInterval);
        //        }
        //        else
        //        {
        //            houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] = new StaticTile(houseMap.GetLayer(layer), sheet, sectionTile.BlendMode, sectionTile.TileIndex);
        //        }

        //        //houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] = new StaticTile(houseMap.GetLayer(layer), sheet, BlendMode.Alpha, sectionTile.TileIndex);
        //        Logger.Log("Checking for properties...");
        //        if (sectionTile.Properties.Keys.Count > 0)
        //        {
        //            Logger.Log("Properties discovered.");
        //            foreach (KeyValuePair<string, PropertyValue> pair in sectionTile.Properties)
        //            {
        //                Logger.Log("Applying property '" + pair.Key + "'...");
        //                houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y].Properties[pair.Key] = pair.Value;
        //                Logger.Log("Applied.");
        //            }
        //        }
        //    }
        //    else if ((!layer.Equals("Back") || destructive) && houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] != null)
        //    {
        //        Logger.Log("Global tile was not null, and local tile was.  Deleting...");
        //        houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y] = null;
        //        Logger.Log("Deleted.");
        //    }
        //}

        //internal static TileSheet getEquivalentSheet(Map houseMap, TileSheet sheet)
        //{
        //    if (sheet.ImageSource.Contains("walls_and_floors"))
        //    {
        //        return houseMap.TileSheets[FarmHouseStates.wallsAndFloorsSheet];
        //    }
        //    else if (sheet.ImageSource.Contains("townInterior"))
        //    {
        //        return houseMap.TileSheets[FarmHouseStates.interiorSheet];
        //    }
        //    else if (sheet.ImageSource.Contains("farmhouse_tiles"))
        //    {
        //        return houseMap.TileSheets[FarmHouseStates.farmSheet];
        //    }
        //    else if (sheet.ImageSource.Contains("SewerTiles") && FarmHouseStates.sewerSheet != -1)
        //    {
        //        Logger.Log("Tile was on sewer sheet!");
        //        return houseMap.TileSheets[FarmHouseStates.sewerSheet];
        //    }
        //    else if (sheet.ImageSource.Contains("SewerTiles"))
        //    {
        //        Logger.Log("Sewer sheet was not set!", LogLevel.Warn);
        //    }
        //    Logger.Log("Could not find an equivalent sheet with the image source '" + sheet.ImageSource + "' on the FarmHouse map.  Using the townInterior sheet...");
        //    return houseMap.TileSheets[FarmHouseStates.interiorSheet];
        //}
    }
}