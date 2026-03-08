using System;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene;

public static class SpriteMovement
{
    public static void ApplyEmotionShake(AnimationContext ctx, (Basis, Emotion) entity)
    {
        var (basis, emotion) = entity;

        basis.offset.X += Interp.Linear(ctx.rng.NextDouble(), 0.0, 1.0, -0.5, 0.5) * emotion.Magnitude * emotion.Magnitude;
        basis.offset.Y += Interp.Linear(ctx.rng.NextDouble(), 0.0, 1.0, -0.5, 0.5) * emotion.Magnitude * emotion.Magnitude;

        basis.offset.X = Math.Clamp(basis.offset.X, -20.0, 20.0);
        basis.offset.Y = Math.Clamp(basis.offset.Y, -20.0, 20.0);
    }

    public static void WrapScreen(AnimationContext ctx, Basis basis)
    {
        var bounds = basis.Bounds;

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
            basis.home.X += (ctx.scene.width + boundsWidth) * (basis.home.X < 0 ? 1 : -1);
        } 

        if (!yInBounds)
        {
            basis.home.Y += (ctx.scene.height + boundsHeight) * (basis.home.Y < 0 ? 1 : -1);
        }
    }

    public static void ClampToScreen(AnimationContext ctx, Basis basis)
    {
        basis.home.X = Math.Clamp(basis.home.X, basis.offset.X, ctx.scene.width + basis.offset.X);
        basis.home.Y = Math.Clamp(basis.home.Y, basis.offset.Y, ctx.scene.height + basis.offset.Y);
    }

    public static void SimulatePhysics(AnimationContext ctx, (Basis, Physics) entity)
    {
        var (basis, physics) = entity;

        basis.home.X += physics.velocity.X * ctx.scene.lastDtMs;
        basis.home.Y += physics.velocity.Y * ctx.scene.lastDtMs;
    }
}
