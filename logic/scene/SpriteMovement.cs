using System;

namespace yoksdotnet.logic.scene;

public static class SpriteMovement
{
    public static void ApplyEmotionShake(AnimationContext ctx, Emotion emotion, PhysicalBasis point)
    {
        point.offset.X += (ctx.rng.NextDouble() * 2.0 - 1.0) * emotion.Magnitude * emotion.Magnitude;
        point.offset.Y += (ctx.rng.NextDouble() * 2.0 - 1.0) * emotion.Magnitude * emotion.Magnitude;

        point.offset.X = Math.Clamp(point.offset.X, -20.0, 20.0);
        point.offset.Y = Math.Clamp(point.offset.Y, -20.0, 20.0);
    }

    public static void WrapScreen(AnimationContext ctx, PhysicalBasis point)
    {
        var bounds = point.Bounds;

        var xInBounds = bounds.bottomRight.X >= 0 && bounds.topLeft.X <= ctx.scene.width;
        var yInBounds = bounds.bottomRight.Y >= 0 && bounds.topLeft.Y <= ctx.scene.height;

        if (yInBounds && xInBounds)
        {
            return;
        }

        var boundsWidth = bounds.bottomRight.X - bounds.topLeft.X;
        var boundsHeight = bounds.bottomRight.Y - bounds.topLeft.Y;

        if (!xInBounds)
        {
            point.home.X += (ctx.scene.width + boundsWidth) * (point.home.X < 0 ? 1 : -1);
        } 

        if (!yInBounds)
        {
            point.home.Y += (ctx.scene.height + boundsHeight) * (point.home.Y < 0 ? 1 : -1);
        }
    }

    public static void ClampToScreen(AnimationContext ctx, PhysicalBasis point)
    {
        point.home.X = Math.Clamp(point.home.X, point.offset.X, ctx.scene.width + point.offset.X);
        point.home.Y = Math.Clamp(point.home.Y, point.offset.Y, ctx.scene.height + point.offset.Y);
    }
}
