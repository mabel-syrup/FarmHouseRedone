using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;

namespace FarmHouseRedone
{
    internal class NPC_setUpForOutdoorPatioActivity_Patch
    {
        public static bool Prefix(NPC __instance)
        {
            Logger.Log("Prefixing " + __instance.Name + "'s patio activity...");

            if (PatioManager.patio == null)
            {
                Logger.Log("Patio not constructed!");
                return true;
            }

            var standingSpot = new Vector2(FarmState.spouseOutdoorLocation.X + PatioManager.patio.offset.X,
                FarmState.spouseOutdoorLocation.Y + PatioManager.patio.offset.Y);

            Game1.warpCharacter(__instance, "Farm", standingSpot);
            __instance.currentMarriageDialogue.Clear();
            __instance.addMarriageDialogue("MarriageDialogue", "patio_" + __instance.Name, false);
            var name = __instance.Name;
            __instance.setTilePosition((int) standingSpot.X, (int) standingSpot.Y);

            var shouldPlaySpousePatioAnimation =
                FarmHouseStates.reflector.GetField<NetBool>(__instance, "shouldPlaySpousePatioAnimation");

            shouldPlaySpousePatioAnimation.GetValue().Value = true;
            return false;
        }
    }
}