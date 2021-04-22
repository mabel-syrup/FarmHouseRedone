using System.Collections.Generic;
using Harmony;

namespace FarmHouseRedone
{
    internal class FarmHouse_getBedSpot_patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Logger.Log("Patching bed location...");
            return instructions;
        }

        //public static void Postfix(Point __result, FarmHouse __instance)
        //{
        //    try
        //    {
        //        Logger.Log("Getting bed location...");
        //        if (FarmHouseStates.bedData == null)
        //        {
        //            Logger.Log("Bed was null, updating...");
        //            FarmHouseStates.updateFromMapPath(__instance.mapPath);
        //            Logger.Log("Updated.");
        //        }
        //        if (FarmHouseStates.bedData != "")
        //        {
        //            Logger.Log("Map defined bed location...");
        //            string[] bedPoint = FarmHouseStates.bedData.Split(' ');
        //            __result = new Point(Convert.ToInt32(bedPoint[0]), Convert.ToInt32(bedPoint[1]));
        //            Logger.Log("Bed set to " + __result.ToString());
        //        }
        //        else
        //        {
        //            Logger.Log("Map did not define a bed location, using vanilla.");
        //            //Run the vanilla code
        //            return;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Logger.Log("Ran.");
        //    }
        //}

        //public static bool Prefix(Point __result, FarmHouse __instance)
        //{
        //    Logger.Log("Getting bed location...");
        //    if (FarmHouseStates.bedData == null)
        //    {
        //        Logger.Log("Bed was null, updating...");
        //        FarmHouseStates.updateFromMapPath(__instance.mapPath);
        //        Logger.Log("Updated.");
        //    }
        //    if (FarmHouseStates.bedData != "")
        //    {
        //        Logger.Log("Map defined bed location...");
        //        string[] bedPoint = FarmHouseStates.bedData.Split(' ');
        //        __result = new Point(Convert.ToInt32(bedPoint[0]), Convert.ToInt32(bedPoint[1]));
        //        Logger.Log("Bed set to " + __result.ToString());
        //        return false;
        //    }
        //    else
        //    {
        //        Logger.Log("Map did not define a bed location, using vanilla.");
        //        //Run the vanilla code
        //        return true;
        //    }
        //}
    }
}