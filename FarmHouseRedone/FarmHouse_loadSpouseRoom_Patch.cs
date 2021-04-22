using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using xTile;
using xTile.ObjectModel;
using xTile.Tiles;

namespace FarmHouseRedone
{
    internal class FarmHouse_loadSpouseRoom_Patch
    {
        internal static bool Prefix(FarmHouse __instance)
        {
            if (__instance is Cabin)
                return true;
            var spouseRoomRects = new List<Rectangle>();
            var state = FarmHouseStates.getState(__instance);
            if (state.spouseRoomData == null)
                FarmHouseStates.updateFromMapPath(__instance, __instance.mapPath.Value);
            if (state.spouseRoomData != "")
            {
                var spousePoint = state.spouseRoomData.Split(' ');
                if (spousePoint.Length < 2)
                {
                    Logger.Log(
                        "Spouse room was defined, but did not have at least two numerical values!  Given '" +
                        state.spouseRoomData + "'.", LogLevel.Error);
                    return true;
                }

                try
                {
                    applyAllSpouseRooms(__instance, spousePoint);
                }
                catch (FormatException)
                {
                    Logger.Log(
                        "Spouse room was defined, but its values do not seem to be numerical!  Given '" +
                        state.spouseRoomData + "'.", LogLevel.Error);
                    return true;
                }
            }
            else
            {
                Logger.Log("No spouse room location was defined.");
                if (FarmHouseStates.spouseRooms.Keys.Contains(Game1.player.spouse))
                    return true;
                Logger.Log(
                    "Spouse was not a base-game marriage candidate, applying vanilla-like spouse room behavior for modded spouse '" +
                    Game1.player.spouse + "'...");
                makeSpouseRoom(__instance, __instance.upgradeLevel == 1 ? 29 : 35,
                    __instance.upgradeLevel == 1 ? 1 : 10, true, true, true, true, new List<string>(),
                    new List<string>(), "", false, new CharacterArguments());
            }

            return false;
        }

        internal static void applyAllSpouseRooms(FarmHouse __instance, string[] spouseRoomData)
        {
            var readerIndex = 0;
            var floorFlag = true;
            var wallFlag = true;
            var clutterFlag = true;
            var windowFlag = true;
            var nameWhitelist = new List<string>();
            var nameBlacklist = new List<string>();
            var nameOverride = "";
            var args = new CharacterArguments();
            var destructive = false;
            var x = -1;
            var y = -1;
            while (readerIndex < spouseRoomData.Length)
                if (isNumeric(spouseRoomData[readerIndex]) && isNumeric(spouseRoomData[readerIndex + 1]))
                {
                    if (x != -1 && y != -1)
                    {
                        Logger.Log("Found spouse room definition: " + x + " " + y +
                                   (floorFlag ? " flooring " : " no flooring ") +
                                   (wallFlag ? " walls " : " no walls ") + (clutterFlag ? " clutter" : " no clutter"));
                        makeSpouseRoom(__instance, x, y, floorFlag, wallFlag, windowFlag, clutterFlag, nameWhitelist,
                            nameBlacklist, nameOverride, destructive, args);
                        floorFlag = true;
                        wallFlag = true;
                        clutterFlag = true;
                        windowFlag = true;
                        nameWhitelist.Clear();
                        nameBlacklist.Clear();
                        nameOverride = "";
                        args = new CharacterArguments();
                        destructive = false;
                        x = -1;
                        y = -1;
                    }

                    x = Convert.ToInt32(spouseRoomData[readerIndex]);
                    y = Convert.ToInt32(spouseRoomData[readerIndex + 1]);
                    readerIndex += 2;
                }
                else if (x == -1 || y == -1)
                {
                    Logger.Log(
                        "Improper spouse room data!  No coordinates appear to be present!  Please ensure all spouse room definitions begin with an X and Y coordinate.",
                        LogLevel.Error);
                    readerIndex++;
                }
                else
                {
                    if (spouseRoomData[readerIndex].StartsWith("-"))
                    {
                        var flag = spouseRoomData[readerIndex].TrimStart('-').ToLower();
                        Logger.Log("Found flag: " + flag);
                        if (flag.StartsWith("wi"))
                        {
                            Logger.Log("Interpreted as 'no window' flag.");
                            windowFlag = false;
                        }
                        else if (flag.StartsWith("w"))
                        {
                            Logger.Log("Interpreted as 'no wall' flag.");
                            wallFlag = false;
                        }
                        else if (flag.StartsWith("fu"))
                        {
                            Logger.Log("Interpreted as 'no furniture' flag.");
                            clutterFlag = false;
                        }
                        else if (flag.StartsWith("f"))
                        {
                            Logger.Log("Interpreted as 'no floor' flag.");
                            floorFlag = false;
                        }
                        else if (flag.StartsWith("c"))
                        {
                            Logger.Log("Interpreted as 'no clutter' flag.");
                            clutterFlag = false;
                        }
                        else if (flag.StartsWith("d"))
                        {
                            Logger.Log("Interpreted as 'destructive' flag.");
                            destructive = true;
                        }
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("+"))
                    {
                        var name = spouseRoomData[readerIndex].TrimStart('+');
                        Logger.Log(name + " was added to the whitelist.");
                        nameWhitelist.Add(name);
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("!"))
                    {
                        var name = spouseRoomData[readerIndex].TrimStart('!');
                        Logger.Log(name + " was added to the blacklist.");
                        nameBlacklist.Add(name);
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("="))
                    {
                        var name = spouseRoomData[readerIndex].TrimStart('=');
                        Logger.Log("Spouse room set to behave as " + name + "'s spouse room (" + name +
                                   "_spouseroom.tbin)");
                        nameOverride = name;
                    }
                    else if (spouseRoomData[readerIndex].StartsWith("%"))
                    {
                        var argumentString = spouseRoomData[readerIndex].TrimStart('%').ToLower();
                        Logger.Log("Character argument detected: " + argumentString);
                        var argument = argumentString.Split(':');
                        if (argument[0].StartsWith("gen"))
                        {
                            var ofSpouse = false;
                            var gender = -1;
                            Logger.Log("Interpreting as gender-based argument...");
                            if (argument[1].StartsWith("p") || argument[1].StartsWith("f"))
                                Logger.Log("Gender of Player argument...");
                            else if (argument[1].StartsWith("s") || argument[1].StartsWith("n") ||
                                     argument[1].StartsWith("h") ||
                                     argument[1].StartsWith("w")) Logger.Log("Gender of Spouse argument...");
                            if (argument[2].StartsWith("m") || argument[2].StartsWith("b"))
                            {
                                Logger.Log("Gender set to Male...");
                                gender = 0;
                            }
                            else if (argument[2].StartsWith("w") || argument[2].StartsWith("g") ||
                                     argument[2].StartsWith("f"))
                            {
                                Logger.Log("Gender set to Female...");
                                gender = 1;
                            }
                            else
                            {
                                Logger.Log("Gender set to Nonbinary...");
                                gender = 2;
                            }

                            if (ofSpouse)
                                args.spouseGender = gender;
                            else
                                args.farmerGender = gender;
                        }

                        if (argument[0].StartsWith("anx") || argument[0].StartsWith("soc"))
                        {
                            Logger.Log("Interpreting as anxiety-based argument...");
                            if (argument[1].StartsWith("s") || argument[1].StartsWith("1"))
                            {
                                Logger.Log("Social anxiety set to Shy...");
                                args.anxiety = 1;
                            }
                            else if (argument[1].StartsWith("o") || argument[1].StartsWith("0"))
                            {
                                Logger.Log("Social anxiety set to Outgoing...");
                                args.anxiety = 0;
                            }
                            else
                            {
                                Logger.Log("Social anxiety set to Neutral...");
                                args.anxiety = -1;
                            }
                        }

                        if (argument[0].StartsWith("man"))
                        {
                            Logger.Log("Interpreting as manners-based argument...");
                            if (argument[1].StartsWith("p") || argument[1].StartsWith("1"))
                            {
                                Logger.Log("Manners set to Polite...");
                                args.manners = 1;
                            }
                            else if (argument[1].StartsWith("o") || argument[1].StartsWith("2"))
                            {
                                Logger.Log("Manners set to Rude...");
                                args.manners = 2;
                            }
                            else
                            {
                                Logger.Log("Manners set to Neutral...");
                                args.manners = -1;
                            }
                        }

                        if (argument[0].StartsWith("opt"))
                        {
                            Logger.Log("Interpreting as optimism-based argument...");
                            if (argument[1].StartsWith("p") || argument[1].StartsWith("1"))
                            {
                                Logger.Log("Optimism set to Positive...");
                                args.optimism = 1;
                            }
                            else if (argument[1].StartsWith("neg") || argument[1].StartsWith("0"))
                            {
                                Logger.Log("Optimism set to Negative...");
                                args.optimism = 0;
                            }
                            else
                            {
                                Logger.Log("Optimism set to Neutral...");
                                args.manners = -1;
                            }
                        }
                    }
                    else
                    {
                        Logger.Log("Unsure what to do with '" + spouseRoomData[readerIndex] + "'...");
                    }

                    readerIndex++;
                }

            if (x != -1 && y != -1)
            {
                Logger.Log("Found spouse room definition: " + x + " " + y +
                           (floorFlag ? " flooring " : " no flooring ") + (wallFlag ? " walls " : " no walls ") +
                           (clutterFlag ? " clutter" : " no clutter"));
                makeSpouseRoom(__instance, x, y, floorFlag, wallFlag, windowFlag, clutterFlag, nameWhitelist,
                    nameBlacklist, nameOverride, destructive, args);
            }
        }

        internal static void makeSpouseRoom(FarmHouse __instance, int x, int y, bool floor, bool wall, bool window,
            bool clutter, List<string> whiteList, List<string> blackList, string nameOverride, bool destructive,
            CharacterArguments args)
        {
            //bool hadAcceptableSpouse = false;
            //bool hadOnlyBlacklistSpouse = true;
            //if (whiteList.Count == 0)
            //    hadAcceptableSpouse = true;
            //if (blackList.Count == 0)
            //    hadOnlyBlacklistSpouse = false;
            //If there is no farmer gender criterion, the farmer is male and the farmer had to be male, or the farmer is female and the farmer had to be female
            if (args.farmerGender == -1 || __instance.owner.IsMale == (args.farmerGender == 0))
            {
                Logger.Log(__instance.owner.Name + " met player gender criteria...");
            }
            else
            {
                Logger.Log(__instance.owner.Name + " did not meet gender criteria.  Gender requirement: " +
                           (args.farmerGender == -1 ? "none set" : args.farmerGender == 0 ? "male" : "female") +
                           ". Farmer gender: " + (__instance.owner.IsMale ? "male" : "female"));
                return;
            }

            var acceptableSpouses = new List<NPC>();

            foreach (var npcName in FarmHouseStates.GetAllCharacterNames(true))
            {
                var npc = Game1.getCharacterFromName(npcName);
                if (npc.isMarried() && npc.getSpouse() == __instance.owner)
                {
                    Logger.Log(npc.Name + " is married to " + __instance.owner.Name);
                    if (whiteList.Count > 0 && whiteList.Contains(npc.Name))
                    {
                        Logger.Log(nameOverride + " was in the whitelist!");
                        acceptableSpouses.Add(npc);
                        //hadAcceptableSpouse = true;
                    }

                    if ((whiteList.Count == 0 || whiteList.Contains(npc.Name)) && !blackList.Contains(npc.Name))
                    {
                        Logger.Log(nameOverride + " was not in the blacklist!");
                        acceptableSpouses.Add(npc);
                        //hadOnlyBlacklistSpouse = false;
                    }
                }
                else
                {
                    Logger.Log(npc.Name + " was not married to " + __instance.owner.Name);
                }
            }

            var fitSpouseCriteria = false;
            foreach (var candidate in acceptableSpouses)
            {
                if (args.spouseGender == -1 || candidate.Gender == args.spouseGender)
                {
                    Logger.Log(candidate.Name + " met gender criteria...");
                }
                else
                {
                    Logger.Log(candidate.Name + "did not meet gender criteria.  Gender requirement: " +
                               (args.spouseGender == -1 ? "none set" :
                                   args.spouseGender == 0 ? "male" :
                                   args.spouseGender == 1 ? "female" : "nonbinary") + ". Spouse gender: " +
                               (candidate.Gender == 0 ? "male" : candidate.Gender == 1 ? "female" : "nonbinary"));
                    continue;
                }

                //Anxiety
                if (args.anxiety == -99 || candidate.SocialAnxiety == args.anxiety || args.anxiety == -1 &&
                    (candidate.SocialAnxiety < 0 || candidate.SocialAnxiety > 1))
                {
                    Logger.Log(candidate.Name + " met anxiety criteria...");
                }
                else
                {
                    Logger.Log(candidate.Name + "did not meet anxiety criteria.  Anxiety requirement: " +
                               (args.anxiety == -99 ? "not set" :
                                   args.anxiety == 0 ? "outgoing" :
                                   args.anxiety == 1 ? "shy" : "neutral") + ". " + candidate.Name + "'s anxiety: " +
                               (candidate.SocialAnxiety == 0 ? "outgoing" :
                                   candidate.SocialAnxiety == 1 ? "shy" : "neutral"));
                    continue;
                }

                //Manners
                if (args.manners == -99 || candidate.Manners == args.manners ||
                    args.manners == -1 && (candidate.Manners < 1 || candidate.Manners > 2))
                {
                    Logger.Log(candidate.Name + " met manners criteria...");
                }
                else
                {
                    Logger.Log(candidate.Name + "did not meet manners criteria.  Manners requirement: " +
                               (args.manners == -99 ? "not set" :
                                   args.manners == 1 ? "polite" :
                                   args.anxiety == 2 ? "rude" : "neutral") + ". " + candidate.Name + "'s manners: " +
                               (candidate.Manners == 1 ? "polite" : candidate.Manners == 2 ? "rude" : "neutral"));
                    continue;
                }

                //Optimism
                if (args.optimism == -99 || candidate.Optimism == args.optimism ||
                    args.optimism == -1 && (candidate.Optimism < 0 || candidate.Optimism > 1))
                {
                    Logger.Log(candidate.Name + " met optimism criteria...");
                }
                else
                {
                    Logger.Log(candidate.Name + "did not meet optimism criteria.  Optimism requirement: " +
                               (args.optimism == -99 ? "not set" :
                                   args.optimism == 0 ? "negative" :
                                   args.optimism == 1 ? "positive" : "neutral") + ". " + candidate.Name +
                               "'s optimism: " +
                               (candidate.Optimism == 0 ? "negative" :
                                   candidate.Optimism == 1 ? "positive" : "neutral"));
                    continue;
                }

                fitSpouseCriteria = true;
            }

            if (acceptableSpouses.Count == 0 || !fitSpouseCriteria)
                return;
            var spouse = __instance.owner.getSpouse();
            if (spouse == null)
            {
                Logger.Log("Had no spouse!");
                return;
            }

            var spouseName = spouse.Name;
            if (nameOverride != null && nameOverride != "")
            {
                Logger.Log("Applying name override of " + nameOverride);
                spouseName = nameOverride;
            }

            FarmHouseStates.clearAll();

            pasteSpouseRoom(__instance, spouseName, new Rectangle(x, y, 6, 9), floor, wall, window, clutter,
                destructive);
        }

        internal static bool isNumeric(string candidate)
        {
            int n;
            return int.TryParse(candidate, out n);
        }

        internal static void pasteSpouseRoom(FarmHouse __instance, string spouse, Rectangle spouseRoomRect, bool floor,
            bool wall, bool window, bool clutter, bool destructive)
        {
            var num = FarmHouseStates.getSpouseRoom(spouse);
            var map = FarmHouseStates.loadSpouseRoomIfPresent(spouse);
            Point point;
            if (map == null)
            {
                if (num == -1)
                {
                    Logger.Log("No spouse room could be found for " + spouse + "!");
                    return;
                }

                map = Game1.game1.xTileContent.Load<Map>("Maps\\spouseRooms");
                point = new Point(num % 5 * 6, num / 5 * 9);
            }
            else
            {
                point = new Point(0, 0);
            }

            __instance.map.Properties.Remove("DayTiles");
            __instance.map.Properties.Remove("NightTiles");

            //mergeMaps(map, point, __instance, spouseRoomRect);

            var adjustMapLightPropertiesForLamp =
                FarmHouseStates.reflector.GetMethod(__instance, "adjustMapLightPropertiesForLamp");

            foreach (var sheet in __instance.map.TileSheets)
            {
                var tileWidth = sheet.SheetWidth;
                var contentSource = sheet.ImageSource;
                Logger.Log("Looking for " + contentSource + "...");
                var pixelHeight = FarmHouseStates.loader.Load<Texture2D>(contentSource, ContentSource.GameContent)
                    .Height;
                sheet.SheetSize = new xTile.Dimensions.Size(tileWidth, pixelHeight / 16);
                Logger.Log("Found " + sheet.SheetSize.Width * sheet.SheetSize.Height + " tiles for " + sheet.Id +
                           ", with a width of " + tileWidth + " and a height of " + (int) (pixelHeight / 16) + ".");
            }

            var equivalentSheets = MapUtilities.SheetHelper.getEquivalentSheets(__instance, map);

            for (var index1 = 0; index1 < spouseRoomRect.Width; ++index1)
            for (var index2 = 0; index2 < spouseRoomRect.Height; ++index2)
            {
                pasteBackLayer(equivalentSheets, __instance.map, map, spouseRoomRect, point, index1, index2, floor,
                    wall, clutter, destructive);
                addProperties(map, __instance.Map, new Point(spouseRoomRect.X, spouseRoomRect.Y), "Back", index1,
                    index2);
                pasteBuildingsLayer(equivalentSheets, __instance.map, map, spouseRoomRect, point, index1, index2,
                    adjustMapLightPropertiesForLamp, floor, wall, window, clutter, destructive);
                addProperties(map, __instance.Map, new Point(spouseRoomRect.X, spouseRoomRect.Y), "Buildings", index1,
                    index2);
                pasteFrontLayer(equivalentSheets, __instance.map, map, spouseRoomRect, point, index1, index2,
                    adjustMapLightPropertiesForLamp, floor, wall, window, clutter, destructive);
                addProperties(map, __instance.Map, new Point(spouseRoomRect.X, spouseRoomRect.Y), "Front", index1,
                    index2);
                //if (map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2] != null)
                //    __instance.map.GetLayer("Back").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)new StaticTile(__instance.map.GetLayer("Back"), __instance.map.TileSheets[getEquivalentSheet(map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2])], BlendMode.Alpha, map.GetLayer("Back").Tiles[point.X + index1, point.Y + index2].TileIndex);
                //if (map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2] != null)
                //{
                //    __instance.map.GetLayer("Buildings").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)new StaticTile(__instance.map.GetLayer("Buildings"), __instance.map.TileSheets[getEquivalentSheet(map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2], true)], BlendMode.Alpha, map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].TileIndex);
                //    adjustMapLightPropertiesForLamp.Invoke(map.GetLayer("Buildings").Tiles[point.X + index1, point.Y + index2].TileIndex, spouseRoomRect.X + index1, spouseRoomRect.Y + index2, "Buildings");
                //}
                //else
                //    __instance.map.GetLayer("Buildings").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)null;
                //if (index2 < spouseRoomRect.Height - 1 && map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2] != null)
                //{
                //    __instance.map.GetLayer("Front").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)new StaticTile(__instance.map.GetLayer("Front"), __instance.map.TileSheets[getEquivalentSheet(map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2])], BlendMode.Alpha, map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2].TileIndex);
                //    adjustMapLightPropertiesForLamp.Invoke(map.GetLayer("Front").Tiles[point.X + index1, point.Y + index2].TileIndex, spouseRoomRect.X + index1, spouseRoomRect.Y + index2, "Front");
                //}
                //else if (index2 < spouseRoomRect.Height - 1)
                //    __instance.map.GetLayer("Front").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2] = (Tile)null;
                if (index1 == 4 && index2 == 4 && !__instance.map.GetLayer("Back")
                    .Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2].Properties.ContainsKey("NoFurniture"))
                    __instance.map.GetLayer("Back").Tiles[spouseRoomRect.X + index1, spouseRoomRect.Y + index2]
                        .Properties.Add(new KeyValuePair<string, PropertyValue>("NoFurniture", new PropertyValue("T")));
            }

            var spousePoint = new Point(spouseRoomRect.X, spouseRoomRect.Y);

            //Combine properties
            foreach (var pair in map.Properties)
            {
                var value = pair.Value;
                var propertyName = pair.Key;
                Logger.Log("Spouse room map contained Properties data, applying now...");
                if (propertyName.Equals("Warp"))
                {
                    //propertyName = "Warp";
                    Logger.Log("Adjusting warp property...");
                    var warpParts = Utility.cleanup(value.ToString()).Split(' ');
                    var warpShifted = "";
                    for (var index = 0; index < warpParts.Length; index += 5)
                        try
                        {
                            Logger.Log("Relative warp found: " + warpParts[index + 0] + " " + warpParts[index + 1] +
                                       " " + warpParts[index + 2] + " " + warpParts[index + 3] + " " +
                                       warpParts[index + 4]);
                            var xi = -1;
                            var yi = -1;
                            var xii = -1;
                            var yii = -1;
                            if (warpParts[0].StartsWith("~"))
                                xi = spousePoint.X + Convert.ToInt32(warpParts[index + 0].TrimStart('~'));
                            else
                                xi = Convert.ToInt32(warpParts[0]);
                            if (warpParts[1].StartsWith("~"))
                                yi = spousePoint.Y + Convert.ToInt32(warpParts[index + 1].TrimStart('~'));
                            else
                                yi = Convert.ToInt32(warpParts[1]);
                            if (warpParts[3].StartsWith("~"))
                                xii = spousePoint.X + Convert.ToInt32(warpParts[index + 3].TrimStart('~'));
                            else
                                xii = Convert.ToInt32(warpParts[3]);
                            if (warpParts[4].StartsWith("~"))
                                yii = spousePoint.Y + Convert.ToInt32(warpParts[index + 4].TrimStart('~'));
                            else
                                yii = Convert.ToInt32(warpParts[4]);
                            var returnWarp = xi + " " + yi + " " + warpParts[index + 2] + " " + xii + " " + yii + " ";
                            Logger.Log("Relative warp became " + returnWarp);
                            warpShifted += returnWarp;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Logger.Log(
                                "Incomplete warp definition found!  Please ensure all warp definitions are formatted as 'X Y map X Y'",
                                LogLevel.Warn);
                        }
                        catch (FormatException)
                        {
                            Logger.Log(
                                "Invalid warp definition found!  Please ensure all warp definitions use numbers fro the X and Y coordinates!",
                                LogLevel.Warn);
                        }

                    value = warpShifted.Trim(' ');
                    Logger.Log("Warp property is now " + value.ToString());
                }

                if (!__instance.map.Properties.ContainsKey(propertyName))
                {
                    Logger.Log("FarmHouse map did not have a '" + propertyName + "' property, setting it to '" +
                               propertyName + "'...");
                    __instance.map.Properties.Add(propertyName, value);
                }
                else
                {
                    var houseValue = __instance.map.Properties[propertyName];
                    Logger.Log("Farmhouse map already had a '" + propertyName + "' value, appending...");
                    __instance.map.Properties[propertyName] =
                        (houseValue.ToString() + " " + value.ToString()).Trim(' ');
                    Logger.Log(propertyName + " is now " + __instance.map.Properties[propertyName].ToString());
                }
            }
        }

        internal static void addProperties(Map sectionMap, Map houseMap, Point housePoint, string layer, int x, int y)
        {
            if (sectionMap.GetLayer(layer).Tiles[x, y] != null)
            {
                Logger.Log("Checking for properties...");
                if (sectionMap.GetLayer(layer).Tiles[x, y].Properties.Keys.Count > 0)
                {
                    Logger.Log("Properties discovered.");
                    foreach (var pair in sectionMap.GetLayer(layer).Tiles[x, y].Properties)
                    {
                        Logger.Log("Applying property '" + pair.Key + "' to tile " +
                                   new Vector2(housePoint.X + x, housePoint.Y + y).ToString() + "...");
                        houseMap.GetLayer(layer).Tiles[housePoint.X + x, housePoint.Y + y].Properties[pair.Key] =
                            pair.Value;
                        Logger.Log("Applied.");
                    }
                }
            }
        }

        internal static void pasteFrontLayer(Dictionary<TileSheet, TileSheet> equivalentSheets, Map houseMap,
            Map spouseMap, Rectangle spouseRoomRect, Point spousePoint, int x, int y,
            IReflectedMethod adjustMapLightPropertiesForLamp, bool floor, bool wall, bool window, bool clutter,
            bool destructive)
        {
            if (y < spouseRoomRect.Height - 1 &&
                spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y] != null)
            {
                var sheet = equivalentSheets[
                    spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y].TileSheet];
                //TileSheet sheet = houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y])];
                var tileIndex = spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex;
                var isWall = false;
                var isFloor = false;
                var isWindow = false;
                var isClutter = false;
                if (sheet.ImageSource.Contains("walls_and_floors"))
                {
                    isWall = tileIndex < 336;
                    isFloor = tileIndex >= 336;
                }
                else if (sheet.ImageSource.Contains("townInterior"))
                {
                    isWall = FarmHouseStates.townInteriorWalls.Contains(tileIndex);
                    isFloor = FarmHouseStates.townInteriorFloors.Contains(tileIndex);
                    isWindow = FarmHouseStates.townInteriorWindows.Contains(tileIndex);
                    isClutter = FarmHouseStates.townInteriorWallFurniture.Contains(tileIndex) ||
                                FarmHouseStates.townInteriorFloorFurniture.Contains(tileIndex);
                }

                if (isWall && !wall)
                    return;
                if (isFloor && !floor)
                    return;
                if (isWindow && !window)
                    return;
                if (isClutter && !clutter)
                    return;

                var houseTile = houseMap.GetLayer("Front").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y];
                var spouseTile = spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y];

                if (spouseTile is AnimatedTile)
                {
                    var framesCount = (spouseTile as AnimatedTile).TileFrames.Length;
                    var frames = new StaticTile[framesCount];
                    for (var i = 0; i < framesCount; i++)
                    {
                        var frame = (spouseTile as AnimatedTile).TileFrames[i];
                        frames[i] = new StaticTile(houseMap.GetLayer("Front"), sheet, frame.BlendMode, frame.TileIndex);
                    }

                    houseMap.GetLayer("Front").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] =
                        new AnimatedTile(houseMap.GetLayer("Front"), frames,
                            (spouseTile as AnimatedTile).FrameInterval);
                }
                else
                {
                    houseMap.GetLayer("Front").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] =
                        new StaticTile(houseMap.GetLayer("Front"), sheet, spouseTile.BlendMode, spouseTile.TileIndex);
                }

                //houseMap.GetLayer("Front").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = (Tile)new StaticTile(houseMap.GetLayer("Front"), houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y])], BlendMode.Alpha, spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex);
                adjustMapLightPropertiesForLamp.Invoke(
                    spouseMap.GetLayer("Front").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex,
                    spouseRoomRect.X + x, spouseRoomRect.Y + y, "Front");
            }
            else if (y < spouseRoomRect.Height - 1 || destructive)
            {
                houseMap.GetLayer("Front").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = (Tile) null;
            }
        }

        internal static void pasteBuildingsLayer(Dictionary<TileSheet, TileSheet> equivalentSheets, Map houseMap,
            Map spouseMap, Rectangle spouseRoomRect, Point spousePoint, int x, int y,
            IReflectedMethod adjustMapLightPropertiesForLamp, bool floor, bool wall, bool window, bool clutter,
            bool destructive)
        {
            if (spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y] != null)
            {
                //Get the equivalent tilesheet.
                var sheet = equivalentSheets[
                    spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y].TileSheet];
                //TileSheet sheet = houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y])];
                var tileIndex = spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex;
                var isWall = false;
                var isFloor = false;
                var isWindow = false;
                var isClutter = false;
                if (sheet.ImageSource.Contains("walls_and_floors"))
                {
                    isWall = tileIndex < 336;
                    isFloor = tileIndex >= 336;
                }
                else if (sheet.ImageSource.Contains("townInterior"))
                {
                    isWall = FarmHouseStates.townInteriorWalls.Contains(tileIndex);
                    isFloor = FarmHouseStates.townInteriorFloors.Contains(tileIndex);
                    isWindow = FarmHouseStates.townInteriorWindows.Contains(tileIndex);
                    isClutter = FarmHouseStates.townInteriorWallFurniture.Contains(tileIndex) ||
                                FarmHouseStates.townInteriorFloorFurniture.Contains(tileIndex);
                }

                if (isWall && !wall)
                    return;
                if (isFloor && !floor)
                    return;
                if (isWindow && !window)
                    return;
                if (isClutter && !clutter)
                    return;

                var houseTile = houseMap.GetLayer("Buildings").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y];
                var spouseTile = spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y];

                if (spouseTile is AnimatedTile)
                {
                    var framesCount = (spouseTile as AnimatedTile).TileFrames.Length;
                    var frames = new StaticTile[framesCount];
                    for (var i = 0; i < framesCount; i++)
                    {
                        var frame = (spouseTile as AnimatedTile).TileFrames[i];
                        frames[i] = new StaticTile(houseMap.GetLayer("Buildings"), sheet, frame.BlendMode,
                            frame.TileIndex);
                    }

                    houseMap.GetLayer("Buildings").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] =
                        new AnimatedTile(houseMap.GetLayer("Buildings"), frames,
                            (spouseTile as AnimatedTile).FrameInterval);
                }
                else
                {
                    houseMap.GetLayer("Buildings").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] =
                        new StaticTile(houseMap.GetLayer("Buildings"), sheet, spouseTile.BlendMode,
                            spouseTile.TileIndex);
                }
                /*houseMap.GetLayer("Buildings").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] 
                    = (Tile)new StaticTile(
                        houseMap.GetLayer("Buildings"),
                        houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y], true)],
                        BlendMode.Alpha,
                        spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex
                );
                */

                adjustMapLightPropertiesForLamp.Invoke(
                    spouseMap.GetLayer("Buildings").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex,
                    spouseRoomRect.X + x, spouseRoomRect.Y + y, "Buildings");
            }
            else if (destructive)
            {
                houseMap.GetLayer("Buildings").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = null;
            }
        }

        internal static void pasteBackLayer(Dictionary<TileSheet, TileSheet> equivalentSheets, Map houseMap,
            Map spouseMap, Rectangle spouseRoomRect, Point spousePoint, int x, int y, bool floor, bool wall,
            bool clutter, bool destructive)
        {
            //If the house has a tile on the back layer at the corresponding coordinate
            if (spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y] != null)
            {
                //Get the equivalent tilesheet.
                var sheet = equivalentSheets[
                    spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y].TileSheet];
                //TileSheet sheet = houseMap.TileSheets[getEquivalentSheet(spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y])];
                var tileIndex = spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex;
                var isWall = false;
                var isFloor = false;
                if (sheet.ImageSource.Contains("walls_and_floors"))
                {
                    isWall = tileIndex < 336;
                    isFloor = tileIndex >= 336;
                }
                else if (sheet.ImageSource.Contains("townInterior"))
                {
                    isWall = FarmHouseStates.townInteriorWalls.Contains(tileIndex);
                    isFloor = FarmHouseStates.townInteriorFloors.Contains(tileIndex);
                }

                if (isWall && !wall)
                    return;
                if (isFloor && !floor)
                    return;


                var spouseTile = spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y];

                if (spouseTile is AnimatedTile)
                {
                    var frameInterval = (spouseTile as AnimatedTile).FrameInterval;
                    Logger.Log("Tile at {" + (spousePoint.X + x) + ", " + (spousePoint.Y + y) +
                               "} is animated, and has " + (spouseTile as AnimatedTile).TileFrames.Length +
                               " frames.\nTile has a frame interval of " + frameInterval + " ms");
                    var framesCount = (spouseTile as AnimatedTile).TileFrames.Length;
                    var frames = new StaticTile[framesCount];
                    for (var i = 0; i < framesCount; i++)
                    {
                        var frame = (spouseTile as AnimatedTile).TileFrames[i];
                        Logger.Log("Adding frame " + i + ": " + frame.TileIndex);
                        frames[i] = new StaticTile(houseMap.GetLayer("Back"), sheet, frame.BlendMode, frame.TileIndex);
                    }

                    houseMap.GetLayer("Back").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] =
                        new AnimatedTile(houseMap.GetLayer("Back"), frames, frameInterval);
                    var houseTile =
                        (AnimatedTile) houseMap.GetLayer("Back").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y];
                    Logger.Log("House tile set to new animated tile with " + houseTile.TileFrames +
                               " frames, and a frame interval of " + houseTile.FrameInterval);
                }
                else
                {
                    houseMap.GetLayer("Back").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] =
                        new StaticTile(houseMap.GetLayer("Back"), sheet, spouseTile.BlendMode, spouseTile.TileIndex);
                }
                /*houseMap.GetLayer("Back").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y]
                    = new StaticTile(
                        houseMap.GetLayer("Back"),
                        sheet,
                        BlendMode.Alpha,
                        spouseMap.GetLayer("Back").Tiles[spousePoint.X + x, spousePoint.Y + y].TileIndex
                );
                */
            }
            else if (destructive)
            {
                houseMap.GetLayer("Back").Tiles[spouseRoomRect.X + x, spouseRoomRect.Y + y] = null;
            }
        }

        //internal static int getEquivalentSheet(Tile tile, FarmHouseState state, bool crybaby = false)
        //{
        //    if (tile == null)
        //        return 0;
        //    string tileSheetId = tile.TileSheet.Id;
        //    string tileSheetSourceImage = tile.TileSheet.ImageSource;
        //    if (tileSheetSourceImage.Contains("walls_and_floors"))
        //    {
        //        if (crybaby)
        //            Logger.Log(tileSheetId + " appears to be equivalent to walls and floors.");
        //        return state.wallsAndFloorsSheet;
        //    }
        //    if (tileSheetSourceImage.Contains("townInterior"))
        //    {
        //        if (crybaby)
        //            Logger.Log(tileSheetId + " appears to be equivalent to indoor.");
        //        return state.interiorSheet;
        //    }
        //    if (tileSheetSourceImage.Contains("farmhouse_tiles"))
        //    {
        //        if (crybaby)
        //            Logger.Log(tileSheetId + " appears to be equivalent to farmhouse.");
        //        return state.farmSheet;
        //    }
        //    if (crybaby)
        //        Logger.Log(tileSheetId + " appears to have no equivalence.");
        //    return 0;
        //}

        internal static void mergeMaps(Map map, Point point, FarmHouse __instance, Rectangle spouseRoomRect)
        {
            var adjustMapLightPropertiesForLamp =
                FarmHouseStates.reflector.GetMethod(__instance, "adjustMapLightPropertiesForLamp");
            var mergedSheets = mergeSheets(map, __instance);
            for (var index1 = 0; index1 < spouseRoomRect.Width; ++index1)
            for (var index2 = 0; index2 < spouseRoomRect.Height; ++index2)
                pasteIfNotNull(__instance.map, map, new Point(spouseRoomRect.X + index1, spouseRoomRect.Y + index2),
                    new Point(point.X + index1, point.Y + index2), mergedSheets, adjustMapLightPropertiesForLamp);
        }

        internal static void pasteIfNotNull(Map house, Map spouse, Point houseTile, Point spouseTile,
            Dictionary<string, int> sheetDictionary, IReflectedMethod adjustMapLightPropertiesForLamp)
        {
            if (spouse.GetLayer("Back").Tiles[spouseTile.X, spouseTile.Y] != null)
            {
                var spouseTileTile = spouse.GetLayer("Back").Tiles[spouseTile.X, spouseTile.Y];
                house.GetLayer("Back").Tiles[houseTile.X, houseTile.Y] = new StaticTile(house.GetLayer("Back"),
                    house.TileSheets[sheetDictionary[spouseTileTile.TileSheet.Id]], BlendMode.Alpha,
                    spouseTileTile.TileIndex);
            }

            if (spouse.GetLayer("Buildings").Tiles[spouseTile.X, spouseTile.Y] != null)
            {
                var spouseTileTile = spouse.GetLayer("Buildings").Tiles[spouseTile.X, spouseTile.Y];
                house.GetLayer("Buildings").Tiles[houseTile.X, houseTile.Y] = new StaticTile(
                    house.GetLayer("Buildings"), house.TileSheets[sheetDictionary[spouseTileTile.TileSheet.Id]],
                    BlendMode.Alpha, spouseTileTile.TileIndex);
                adjustMapLightPropertiesForLamp.Invoke(
                    spouse.GetLayer("Buildings").Tiles[spouseTile.X, spouseTile.Y].TileIndex, houseTile.X, houseTile.Y,
                    "Buildings");
            }
            else
            {
                house.GetLayer("Buildings").Tiles[houseTile.X, houseTile.Y] = (Tile) null;
            }

            if (spouse.GetLayer("Front").Tiles[spouseTile.X, spouseTile.Y] != null)
            {
                var spouseTileTile = spouse.GetLayer("Front").Tiles[spouseTile.X, spouseTile.Y];
                house.GetLayer("Front").Tiles[houseTile.X, houseTile.Y] = new StaticTile(house.GetLayer("Front"),
                    house.TileSheets[sheetDictionary[spouseTileTile.TileSheet.Id]], BlendMode.Alpha,
                    spouseTileTile.TileIndex);
                adjustMapLightPropertiesForLamp.Invoke(
                    spouse.GetLayer("Front").Tiles[spouseTile.X, spouseTile.Y].TileIndex, houseTile.X, houseTile.Y,
                    "Front");
            }
        }

        internal static Dictionary<string, int> mergeSheets(Map map, FarmHouse __instance)
        {
            var mergedSheets = new Dictionary<string, int>();
            var sheetsToAdd = new List<TileSheet>();
            foreach (var sheet in map.TileSheets)
            {
                foreach (var houseSheet in __instance.map.TileSheets)
                    if (sheet.ImageSource == houseSheet.ImageSource)
                    {
                        Logger.Log("Spouse room had copy of a sheet already within farmhouse, " + sheet.Id + "->" +
                                   houseSheet.Id);
                        mergedSheets[sheet.Id] = __instance.map.TileSheets.IndexOf(houseSheet);
                        break;
                    }

                Logger.Log("Spouse room added a new sheet: " + sheet.Id);
                sheetsToAdd.Add(sheet);
            }

            foreach (var sheet in sheetsToAdd)
            {
                __instance.map.AddTileSheet(sheet);
                mergedSheets[sheet.Id] = __instance.map.TileSheets.IndexOf(sheet);
            }

            return mergedSheets;
        }
    }
}