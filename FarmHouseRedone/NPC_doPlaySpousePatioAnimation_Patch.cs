using StardewValley;

namespace FarmHouseRedone
{
    internal class NPC_doPlaySpousePatioAnimation_Patch
    {
        public static bool Prefix(NPC __instance)
        {
            Logger.Log("Prefixing " + __instance.Name + "'s patio animation...");
            if (PatioManager.patio == null)
                return true;
            Logger.Log("Patio exists...");
            var frames = PatioManager.patio.getAnimation();
            if (frames.Count < 1)
                return true;
            Logger.Log("Patio animation was provided.");
            __instance.Sprite.setCurrentAnimation(frames);
            return false;
        }
    }
}