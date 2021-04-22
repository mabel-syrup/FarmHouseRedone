using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;
using StardewValley.Buildings;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace FarmHouseRedone
{
    internal class Farm_resetLocalState_Patch
    {
        public static void Postfix(Farm __instance)
        {
            var shippingBinLid = FarmHouseStates.reflector
                .GetField<TemporaryAnimatedSprite>(__instance, "shippingBinLid").GetValue();
            var lidPosition =
                new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f +
                new Vector2(2f, -7f) * 4f;
            shippingBinLid.Position = lidPosition;
            shippingBinLid.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1) / 10000f;
        }
    }


    //[HarmonyPriority(Priority.Last)]
}