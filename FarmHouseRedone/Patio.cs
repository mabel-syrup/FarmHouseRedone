using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using xTile;
using xTile.Layers;
using xTile.Display;
using xTile.Tiles;
using xTile.ObjectModel;
using StardewValley;

namespace FarmHouseRedone
{
    public class Patio
    {
        public Vector2 offset;
        public Map patioMap;

        //For testing purposes
        //Final should allow for selectors
        public string whichSpouse;

        public Patio(Vector2 offset, Map patioMap, string whichSpouse)
        {
            this.offset = offset;
            this.patioMap = patioMap;
            this.whichSpouse = whichSpouse;
        }

        public void pasteMap(GameLocation location, int pasteX, int pasteY)
        {
            var equivalentSheets = MapUtilities.SheetHelper.getEquivalentSheets(location, patioMap);
            var mapSize = getMapSize(patioMap);
            for (var x = 0; x < mapSize.X; x++)
            for (var y = 0; y < mapSize.Y; y++)
                MapUtilities.MapMerger.pasteTile(location.map, patioMap, x, y, pasteX + x, pasteY + y,
                    equivalentSheets);
        }

        private Vector2 getMapSize(Map map)
        {
            return new Vector2(map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
        }

        public List<FarmerSprite.AnimationFrame> getAnimation()
        {
            Logger.Log("Getting patio animation for Patio_" + whichSpouse + "...");
            var outFrames = new List<FarmerSprite.AnimationFrame>();
            if (patioMap == null || !patioMap.Properties.ContainsKey("Animation")) return outFrames;
            var framesProperty = Utility.cleanup(patioMap.Properties["Animation"]).Split(' ');
            for (var frame = 0; frame < framesProperty.Length - 1; frame += 2)
                try
                {
                    var frameIndex = Convert.ToInt32(framesProperty[frame]);
                    var frameDuration = Convert.ToInt32(framesProperty[frame + 1]);
                    outFrames.Add(new FarmerSprite.AnimationFrame(frameIndex, frameDuration));
                }
                catch (FormatException)
                {
                    Logger.Log("Animation frame formatting incorrect!  Incorrect frame: index=" +
                               framesProperty[frame] + ", duration=" + framesProperty[frame + 1] +
                               ".\nFull animation string: " + Utility.cleanup(patioMap.Properties["Animation"]));
                }

            Logger.Log("Added " + outFrames.Count + " frames");
            return outFrames;
        }
    }
}