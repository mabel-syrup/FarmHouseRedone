using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone
{
    internal class Shed_getWalls_Patch
    {
        public static void Postfix(ref List<Rectangle> __result, Shed __instance)
        {
            __result.Clear();

            var state = OtherLocations.DecoratableStates.getState(__instance);

            __result = state.getWalls();

            if (__result.Count > 0)
                return;
            else
                __result = new List<Rectangle>()
                {
                    new Rectangle(1, 1, 11, 3)
                };
        }
    }
}