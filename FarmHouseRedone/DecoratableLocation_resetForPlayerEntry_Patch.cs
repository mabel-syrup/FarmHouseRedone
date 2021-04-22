using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Locations;
using Harmony;

namespace FarmHouseRedone
{
    internal class DecoratableLocation_resetForPlayerEntry_Patch
    {
        internal static bool Prefix(DecoratableLocation __instance)
        {
            MapUtilities.FacadeHelper.setWallpaperDefaults(__instance);
            //if (__instance.Name.StartsWith("DECORHOST_"))
            //{

            //}
            return true;
        }
    }


    //This sets the data for the chosen wall to reflect the index of the wallpaper used.
    //This does _not_ set the tiles.
}