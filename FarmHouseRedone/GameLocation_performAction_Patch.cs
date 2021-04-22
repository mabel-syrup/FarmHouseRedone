using StardewValley;
using xTile.Dimensions;

namespace FarmHouseRedone
{
    internal class GameLocation_performAction_Patch
    {
        public static bool Prefix(string action, Farmer who, Location tileLocation, bool __result,
            GameLocation __instance)
        {
            if (action == "Mailbox")
            {
                Logger.Log(Game1.player.Name + " checked the mailbox at " + tileLocation.ToString() + "...");
                if (__instance is Farm)
                {
                    Logger.Log("Mailbox was on the farm...");
                    var mailboxPosition = FarmState.getMailboxPosition(Game1.player);
                    Logger.Log(Game1.player.Name + "'s mailbox is at " + mailboxPosition.ToString());
                    if (tileLocation.X != mailboxPosition.X || tileLocation.Y != mailboxPosition.Y)
                    {
                        Logger.Log("Mailbox did not belong to " + Game1.player.Name);
                        return true;
                    }

                    Logger.Log("Mailbox belonged to " + Game1.player.Name);
                    __instance.mailbox();
                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}