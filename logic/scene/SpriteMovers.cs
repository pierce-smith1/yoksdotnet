using System;
using System.Collections.Generic;

using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public delegate void MoveFunction(Scene scene, Sprite sprite, IEnumerable<Sprite> allSprites);

public partial class MoverState
{
    public enum OffscreenBehaviors
    {
        Wrap,
        Clamp,
    }

    public OffscreenBehaviors OffscreenBehavior { get; set; } = OffscreenBehaviors.Wrap;
}

public static class SpriteMovers
{
    public static void OffscreenMover(Scene scene, Sprite sprite, IEnumerable<Sprite> allSprites)
    {
        var bounds = sprite.GetBounds();

        var xInBounds = bounds.BotRight.X >= 0 && bounds.TopLeft.X <= scene.Width;
        var yInBounds = bounds.BotRight.Y >= 0 && bounds.TopLeft.Y <= scene.Height;

        if (yInBounds && xInBounds) {
            // Our sight is seen -
            // We are not offscreen!
            return;
        }

        switch (sprite.MoverState.OffscreenBehavior)
        {
            case MoverState.OffscreenBehaviors.Wrap:
                var boundsWidth = bounds.BotRight.X - bounds.TopLeft.X;
                var boundsHeight = bounds.BotRight.Y - bounds.TopLeft.Y;

                if (!xInBounds)
                {
                    sprite.Home.X += (scene.Width + boundsWidth) * (sprite.Home.X < 0 ? 1 : -1);
                } 

                if (!yInBounds)
                {
                    sprite.Home.Y += (scene.Height + boundsHeight) * (sprite.Home.Y < 0 ? 1 : -1);
                }

                break;

            case MoverState.OffscreenBehaviors.Clamp:
                sprite.Home.X = Math.Clamp(sprite.Home.X, sprite.Offset.X, scene.Width + sprite.Offset.X);
                sprite.Home.Y = Math.Clamp(sprite.Home.Y, sprite.Offset.Y, scene.Width + sprite.Offset.Y);

                break;
        }
    }

    public static void YokinShakeMover(Scene scene, Sprite sprite, IEnumerable<Sprite> allSprites)
    {
        if (sprite is not Yokin yokin)
        {
            return;
        }

        var emotionMagnitude = yokin.GetEmotionStrength();
        var rng = RandomUtils.SharedRng;

        sprite.Offset.X += (rng.NextDouble() * 2 - 1) * Math.Sqrt(emotionMagnitude);
        sprite.Offset.Y += (rng.NextDouble() * 2 - 1) * Math.Sqrt(emotionMagnitude);

        sprite.Offset.X = Math.Clamp(sprite.Offset.X, -20, 20);
        sprite.Offset.Y = Math.Clamp(sprite.Offset.Y, -20, 20);
    }
}
