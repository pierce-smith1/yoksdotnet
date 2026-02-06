using System;

using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene.patterns;

public record Pattern(string Name, string Description, Action<AnimationContext> MoveFunction) : ISfEnum
{
    public readonly static Pattern Lattice = new(
        "Lattice", 
        "No motion",
        PatternMovement.MoveLattice
    );

    public readonly static Pattern Roamers = new(
        "Roamers", 
        "Drift from left to right, softly bobbing up and down",
        PatternMovement.MoveRoamers
    );

    public readonly static Pattern Waves = new(
        "Waves", 
        "Move in large, random circles",
        PatternMovement.MoveWaves
    );

    public readonly static Pattern Lissajous = new(
        "Lissajous",
        "Line up and march in a snaking curve",
        PatternMovement.MoveLissajous
    );

    public override string ToString() => Name;
}

public static class PatternMovement
{
    public static void MoveLattice(AnimationContext ctx) { }

    public static void MoveRoamers(AnimationContext ctx)
    {
        foreach (var sprite in ctx.scene.sprites)
        {
            sprite.home.X += sprite.brand * ctx.scene.lastDtMs;
            sprite.home.Y += Math.Sin(ctx.scene.seconds * sprite.brand) * ctx.scene.lastDtMs;

            SpriteMovement.WrapSprite(ctx, sprite);
        }
    }

    public static void MoveWaves(AnimationContext ctx)
    {
        foreach (var sprite in ctx.scene.sprites)
        {
            sprite.home.X += Math.Sin(ctx.scene.seconds * sprite.brand) * ctx.scene.lastDtMs;
            sprite.home.Y += Math.Cos(ctx.scene.seconds * sprite.brand) * ctx.scene.lastDtMs;

            SpriteMovement.WrapSprite(ctx, sprite);
        }
    }

    public static void MoveLissajous(AnimationContext ctx)
    {
        foreach (var sprite in ctx.scene.sprites)
        {
            var minX = Math.Max((ctx.scene.width - ctx.scene.height) / 2.0, 0.0);
            var maxX = ctx.scene.width - minX;

            var minY = Math.Max((ctx.scene.height - ctx.scene.width) / 2.0, 0.0);
            var maxY = ctx.scene.height - minY;

            var targetX = Interp.Linear(
                Math.Sin(ctx.scene.seconds * 4.0 - sprite.brand * 17.0),
                -1.0, 1.0,
                minX, maxX - Bitmap.BitmapSize()
            );

            var targetY = Interp.Linear(
                Math.Cos(ctx.scene.seconds * 4.0 - sprite.brand * 11.0),
                -1.0, 1.0,
                minY, maxY - Bitmap.BitmapSize()
            );

            sprite.home.X = targetX + (sprite.home.X - targetX) * 0.9;
            sprite.home.Y = targetY + (sprite.home.Y - targetY) * 0.9;
        }
    }
}
