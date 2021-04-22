using System;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Buildings;
using Harmony;

namespace FarmHouseRedone
{
    [HarmonyPriority(Priority.Last)]
    internal class Game1_warpFarmer_Patch
    {
        public static bool Prefix(LocationRequest locationRequest, int tileX, int tileY, int facingDirectionAfterWarp)
        {
            if (locationRequest.Name.Equals("CABIN_ERROR"))
            {
                Logger.Log("Player attempted to warp the a Cabin, but encountered an error!",
                    StardewModdingAPI.LogLevel.Error);
                return false;
            }
            else if (locationRequest.Name.StartsWith("Cabin"))
            {
                Logger.Log("Player warping to Cabin.  Destination: " + locationRequest.ToString() + ", {" + tileX +
                           ", " + tileY + "}");
            }

            return true;
        }
    }
}