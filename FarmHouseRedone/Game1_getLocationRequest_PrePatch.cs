using System.Collections.Generic;
using Harmony;
using StardewValley;
using StardewValley.Locations;

namespace FarmHouseRedone
{
    [HarmonyPriority(Priority.First)]
    internal class Game1_getLocationRequest_PrePatch
    {
        public static bool Prefix(string locationName, bool isStructure, ref LocationRequest __result)
        {
            if (locationName.ToLower().Equals("here"))
            {
                var name = Game1.currentLocation.isStructure.Value
                    ? Game1.currentLocation.NameOrUniqueName
                    : Game1.currentLocation.Name;
                __result = new LocationRequest(name, Game1.currentLocation.isStructure.Value, Game1.currentLocation);
                Logger.Log("Location request: " + __result.ToString());
                return false;
            }

            if (locationName.Equals("Cabin") && !(Game1.currentLocation is Cabin))
            {
                Logger.Log("Parsing vague destination 'Cabin' as Cabin_0");
                locationName = "Cabin_0";
            }

            if (locationName.Split('_').Length > 1 && locationName.StartsWith("Cabin"))
            {
                Logger.Log("Warping to cabin...");

                var allCabins = getCabins(Game1.getFarm());
                if (allCabins.Count < 1)
                {
                    Logger.Log("No cabins found!");
                    __result = new LocationRequest("CABIN_ERROR", false, null);
                    return false;
                }

                var identifier = locationName.Split('_')[1].ToLower();

                var resultCabin = getCabinForID(identifier, allCabins);

                if (resultCabin != null)
                {
                    Logger.Log("Warping to cabin...");
                    __result = new LocationRequest("Cabin", true, resultCabin);
                    return false;
                }

                Logger.Log("Cabin was null, using default code instead.");
            }

            return true;
        }

        public static Cabin getCabinForID(string identifier, List<Cabin> cabins)
        {
            int cabinID;
            if (int.TryParse(identifier, out cabinID))
            {
                if (cabinID >= cabins.Count)
                    cabinID = 0;
                Logger.Log("Getting cabin #" + cabinID);
                return cabins[cabinID];
            }

            if (identifier.StartsWith("@"))
            {
                Logger.Log("Searching for " + identifier.Substring(1) + "'s cabin...");
                foreach (var cabin in cabins)
                    if (cabin.owner != null && cabin.owner.Name.ToLower().Equals(identifier.TrimStart('@').ToLower()))
                    {
                        Logger.Log("Found " + identifier.Substring(1) + "'s cabin!");
                        return cabin;
                    }
            }

            if (identifier.ToLower().Equals("home"))
            {
                Logger.Log("Searching for " + Game1.player.Name + "'s cabin...");
                foreach (var cabin in cabins)
                    if (cabin.owner != null && cabin.owner == Game1.player)
                    {
                        Logger.Log("Found " + cabin.owner.Name + "'s cabin!");
                        return cabin;
                    }
            }

            Logger.Log(identifier + " did not seem to find any valid cabin, using the first cabin instead...");
            return cabins[0];
        }

        public static List<Cabin> getCabins(BuildableGameLocation location)
        {
            var outCabins = new List<Cabin>();
            foreach (var b in location.buildings)
                if (b.indoors.Value != null && b.indoors.Value is Cabin)
                    outCabins.Add(b.indoors.Value as Cabin);
            return outCabins;
        }
    }
}