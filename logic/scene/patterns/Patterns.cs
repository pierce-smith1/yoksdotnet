using System;

using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene.patterns;

public record Pattern(string Name, string Description) : ISfEnum
{
    public readonly static Pattern Lattice = new(
        "Lattice", 
        "No motion"
    );

    public readonly static Pattern Roamers = new(
        "Roamers", 
        "Drift from left to right, softly bobbing up and down"
    );

    public readonly static Pattern Waves = new(
        "Waves", 
        "Move in large, random circles"
    );

    public readonly static Pattern Lissajous = new(
        "Lissajous",
        "Line up and march in a snaking curve"
    );

    public override string ToString() => Name;
}

public static class PatternMovement
{
    private static Brand _genericBrand = new()
    {
        value = 0.0,
    };

    public static void MoveByPattern(AnimationContext ctx, Pattern pattern, Entity entity)
    {
        if (pattern == Pattern.Lattice)
        {
            return;
        }

        var brand = entity.brand ?? _genericBrand;

        if (pattern == Pattern.Roamers)
        {
            MoveRoamers(ctx, entity.basis, brand);
        }
        else if (pattern == Pattern.Waves)
        {
            MoveWaves(ctx, entity.basis, brand);
        }
        else if (pattern == Pattern.Lissajous)
        {
            MoveLissajous(ctx, entity.basis, brand);
        }
    }

    public static void MoveRoamers(AnimationContext ctx, PhysicalBasis basis, Brand brand)
    {
        basis.home.X += brand.value * ctx.scene.lastDtMs;
        basis.home.Y += Math.Sin(ctx.scene.seconds * brand.value) * ctx.scene.lastDtMs;

        SpriteMovement.WrapScreen(ctx, basis);
    }

    public static void MoveWaves(AnimationContext ctx, PhysicalBasis basis, Brand brand)
    {
        basis.home.X += Math.Sin(ctx.scene.seconds * brand.value) * ctx.scene.lastDtMs;
        basis.home.Y += Math.Cos(ctx.scene.seconds * brand.value) * ctx.scene.lastDtMs;

        SpriteMovement.WrapScreen(ctx, basis);
    }

    public static void MoveLissajous(AnimationContext ctx, PhysicalBasis basis, Brand brand)
    {
        var minX = Math.Max((ctx.scene.width - ctx.scene.height) / 2.0, 0.0);
        var maxX = ctx.scene.width - minX;

        var minY = Math.Max((ctx.scene.height - ctx.scene.width) / 2.0, 0.0);
        var maxY = ctx.scene.height - minY;

        var targetX = Interp.Linear(
            Math.Sin(ctx.scene.seconds * 4.0 - brand.value * 17.0),
            -1.0, 1.0,
            minX, maxX - Bitmap.BitmapSize()
        );

        var targetY = Interp.Linear(
            Math.Cos(ctx.scene.seconds * 4.0 - brand.value * 11.0),
            -1.0, 1.0,
            minY, maxY - Bitmap.BitmapSize()
        );

        basis.home.X = targetX + (basis.home.X - targetX) * 0.9;
        basis.home.Y = targetY + (basis.home.Y - targetY) * 0.9;
    }
}
