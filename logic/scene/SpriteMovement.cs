using System;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public static class SpriteMovement
{
    public static void ApplyEmotionShake(AnimationContext ctx)
    {
        foreach (var sprite in ctx.scene.sprites)
        {
            if (sprite.addons.emotions is not { } emotions)
            {
                continue;
            }

            sprite.offset.X += (ctx.rng.NextDouble() * 2.0 - 1.0) * emotions.Magnitude * emotions.Magnitude;
            sprite.offset.Y += (ctx.rng.NextDouble() * 2.0 - 1.0) * emotions.Magnitude * emotions.Magnitude;

            sprite.offset.X = Math.Clamp(sprite.offset.X, -20.0, 20.0);
            sprite.offset.Y = Math.Clamp(sprite.offset.Y, -20.0, 20.0);
        }
    }

    public static void MoveByCurrentPattern(AnimationContext ctx)
    {
        var pattern = ctx.scene.currentPattern ?? Pattern.Lattice;

        if (pattern == Pattern.Roamers) 
        {
            MoveRoamers(ctx);
        }
        else if (pattern == Pattern.Waves)
        {
            MoveWaves(ctx);
        }
    }

    public static void MoveRoamers(AnimationContext ctx)
    {
        foreach (var sprite in ctx.scene.sprites)
        {
            sprite.home.X += sprite.brand * ctx.scene.lastDtMs;
            sprite.home.Y += Math.Sin(ctx.scene.seconds * sprite.brand) * ctx.scene.lastDtMs;

            WrapSprite(ctx, sprite);
        }
    }

    public static void MoveWaves(AnimationContext ctx)
    {
        foreach (var sprite in ctx.scene.sprites)
        {
            sprite.home.X += Math.Sin(ctx.scene.seconds * sprite.brand) * ctx.scene.lastDtMs;
            sprite.home.Y += Math.Cos(ctx.scene.seconds * sprite.brand) * ctx.scene.lastDtMs;

            WrapSprite(ctx, sprite);
        }
    }

    private static void WrapSprite(AnimationContext ctx, Sprite sprite)
    {
        var bounds = sprite.Bounds;

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
            sprite.home.X += (ctx.scene.width + boundsWidth) * (sprite.home.X < 0 ? 1 : -1);
        } 

        if (!yInBounds)
        {
            sprite.home.Y += (ctx.scene.height + boundsHeight) * (sprite.home.Y < 0 ? 1 : -1);
        }
    }

    private static void ClampSprite(AnimationContext ctx, Sprite sprite)
    {
        sprite.home.X = Math.Clamp(sprite.home.X, sprite.offset.X, ctx.scene.width + sprite.offset.X);
        sprite.home.Y = Math.Clamp(sprite.home.Y, sprite.offset.Y, ctx.scene.height + sprite.offset.Y);
    }
}
