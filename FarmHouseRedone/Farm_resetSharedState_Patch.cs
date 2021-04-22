using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

namespace FarmHouseRedone
{
    internal class Farm_resetSharedState_Patch
    {
        public static void Postfix(Farm __instance)
        {
            var houseSource = FarmHouseStates.reflector.GetField<NetRectangle>(__instance, "houseSource");
            var greenhouseSource = FarmHouseStates.reflector.GetField<NetRectangle>(__instance, "greenhouseSource");
            houseSource.SetValue(new NetRectangle(new Rectangle(0, 0, 0, 0)));
            greenhouseSource.SetValue(new NetRectangle(new Rectangle(0, 0, 0, 0)));
            FarmState.init();
            FarmState.setUpFarm(__instance);
        }
    }
}