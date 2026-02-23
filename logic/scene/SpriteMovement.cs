using System;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public static class SpriteMovement
{
    public static void ApplyEmotionShake(AnimationContext ctx, Emotion emotion, Basis point)
    {
        point.offset.X += Interp.Linear(ctx.rng.NextDouble(), 0.0, 1.0, -0.5, 0.5) * emotion.Magnitude * emotion.Magnitude;
        point.offset.Y += Interp.Linear(ctx.rng.NextDouble(), 0.0, 1.0, -0.5, 0.5) * emotion.Magnitude * emotion.Magnitude;

        point.offset.X = Math.Clamp(point.offset.X, -20.0, 20.0);
        point.offset.Y = Math.Clamp(point.offset.Y, -20.0, 20.0);
    }

    public static void WrapScreen(AnimationContext ctx, Basis point)
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

    public static void ClampToScreen(AnimationContext ctx, Basis basis)
    {
        basis.home.X = Math.Clamp(basis.home.X, basis.offset.X, ctx.scene.width + basis.offset.X);
        basis.home.Y = Math.Clamp(basis.home.Y, basis.offset.Y, ctx.scene.height + basis.offset.Y);
    }

    public static void SimulatePhysics(AnimationContext ctx, Basis basis, Physics physics)
    {
        basis.home.X += physics.velocity.X * ctx.scene.lastDtMs;
        basis.home.Y += physics.velocity.Y * ctx.scene.lastDtMs;
    }
}
