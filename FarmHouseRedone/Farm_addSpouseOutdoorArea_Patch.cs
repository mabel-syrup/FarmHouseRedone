using StardewValley;

namespace FarmHouseRedone
{
    internal class Farm_addSpouseOutdoorArea_Patch
    {
        public static bool Prefix(string spouseName, Farm __instance)
        {
            PatioManager.applyPatio(__instance, spouseName);
            return false;
        }
    }
}