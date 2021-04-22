using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone
{
    internal class Farm_showShipment_Patch
    {
        public static bool Prefix(StardewValley.Object o, bool playThrowSound, Farm __instance)
        {
            if (playThrowSound)
                __instance.localSound("backpackIN");
            DelayedAction.playSoundAfterDelay("Ship", playThrowSound ? 250 : 0, (GameLocation) null);
            var num1 = Game1.random.Next();
            var temporarySprites1 = __instance.temporarySprites;
            var temporaryAnimatedSprite1 = new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                new Rectangle(524, 218, 34, 22),
                new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f +
                new Vector2(0.0f, 5f) * 4f, false, 0.0f, Color.White);
            temporaryAnimatedSprite1.interval = 100f;
            temporaryAnimatedSprite1.totalNumberOfLoops = 1;
            temporaryAnimatedSprite1.animationLength = 3;
            temporaryAnimatedSprite1.pingPong = true;
            temporaryAnimatedSprite1.scale = 4f;
            temporaryAnimatedSprite1.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1f) / 10000f;
            var num2 = (double) num1;
            temporaryAnimatedSprite1.id = (float) num2;
            var num3 = num1;
            temporaryAnimatedSprite1.extraInfoForEndBehavior = num3;
            var endBehavior =
                new TemporaryAnimatedSprite.endBehavior(((GameLocation) __instance).removeTemporarySpritesWithID);
            temporaryAnimatedSprite1.endFunction = endBehavior;
            temporarySprites1.Add(temporaryAnimatedSprite1);
            var temporarySprites2 = __instance.temporarySprites;
            var temporaryAnimatedSprite2 = new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                new Rectangle(524, 230, 34, 10),
                new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f +
                new Vector2(0.0f, 17f) * 4f, false, 0.0f, Color.White);
            temporaryAnimatedSprite2.interval = 100f;
            temporaryAnimatedSprite2.totalNumberOfLoops = 1;
            temporaryAnimatedSprite2.animationLength = 3;
            temporaryAnimatedSprite2.pingPong = true;
            temporaryAnimatedSprite2.scale = 4f;
            temporaryAnimatedSprite2.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1f) / 10000f;
            var num4 = (double) num1;
            temporaryAnimatedSprite2.id = (float) num4;
            var num5 = num1;
            temporaryAnimatedSprite2.extraInfoForEndBehavior = num5;
            temporarySprites2.Add(temporaryAnimatedSprite2);
            var temporarySprites3 = __instance.temporarySprites;
            var temporaryAnimatedSprite3 = new TemporaryAnimatedSprite("Maps\\springobjects",
                Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, o.ParentSheetIndex, 16, 16),
                new Vector2(FarmState.shippingCrateLocation.X, FarmState.shippingCrateLocation.Y - 1) * 64f +
                new Vector2((float) (8 + Game1.random.Next(6)), 2f) * 4f, false, 0.0f, Color.White);
            temporaryAnimatedSprite3.interval = 9999f;
            temporaryAnimatedSprite3.scale = 4f;
            temporaryAnimatedSprite3.alphaFade = 0.045f;
            temporaryAnimatedSprite3.layerDepth = ((FarmState.shippingCrateLocation.Y + 1) * 64 + 1f) / 10000f;
            var vector2_1 = new Vector2(0.0f, 0.3f);
            temporaryAnimatedSprite3.motion = vector2_1;
            var vector2_2 = new Vector2(0.0f, 0.2f);
            temporaryAnimatedSprite3.acceleration = vector2_2;
            var num6 = -0.0500000007450581;
            temporaryAnimatedSprite3.scaleChange = (float) num6;
            temporarySprites3.Add(temporaryAnimatedSprite3);
            return false;
        }
    }
}