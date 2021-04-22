using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection.Emit;
using StardewValley;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone
{
    internal class Event_setUpCharacters_Patch
    {
        public static bool Prefix(ref string description, GameLocation location, Event __instance)
        {
            Logger.Log("Prefixing event at " + location.Name + "...");
            if (!(location is Farm))
                return true;
            Logger.Log("Event is at the farm...");
            var strArray = description.Split(' ');
            var index = 0;
            while (index < strArray.Length)
            {
                if (!strArray[index + 1].Equals("-1"))
                {
                    Logger.Log("Found coordinates for character setup: " + strArray[index + 1] + ", " +
                               strArray[index + 2]);
                    var offset = FarmState.getFrontDoorOffset();
                    var x = Convert.ToInt32(strArray[index + 1]) + (int) offset.X;
                    var y = Convert.ToInt32(strArray[index + 2]) + (int) offset.Y;
                    Logger.Log("Offset coordinates to " + x + ", " + y);
                    strArray[index + 1] = x.ToString();
                    strArray[index + 2] = y.ToString();
                }

                index += 4;
            }

            description = "";
            foreach (var descPart in strArray) description += descPart + " ";
            description = description.Substring(0, description.Length - 1);
            Logger.Log("Returning \n" + description);
            return true;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var indicesToDelete = new List<int>();

            for (var i = 0; i < codes.Count; i++)
                if (codes[i].opcode == OpCodes.Call &&
                    codes[i].operand.ToString().Contains("getFrontDoorPositionForFarmer"))
                {
                    Logger.Log("Replacing vanilla getFrontDoorPositionForFarmer() call at index " + i + "...");
                    codes[i] = new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(FarmState), nameof(FarmState.getFrontDoorSpot)));
                    Logger.Log("Index " + i + ": " + codes[i].ToString());
                    indicesToDelete.Add(i - 2);
                    indicesToDelete.Add(i - 1);
                }

            indicesToDelete.Reverse();

            foreach (var index in indicesToDelete) codes.RemoveAt(index);
            return codes.AsEnumerable();
        }
    }
}