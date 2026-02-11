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

    public readonly static Pattern Bouncy = new(
        "Bouncy",
        "DVD screensaver style"
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
            SimpleMovement.MoveRoamers(ctx, entity.basis, brand);
        }
        else if (pattern == Pattern.Bouncy)
        {
            BouncyPattern.Move(ctx, entity);
        }
        else if (pattern == Pattern.Waves)
        {
            SimpleMovement.MoveWaves(ctx, entity.basis, brand);
        }
        else if (pattern == Pattern.Lissajous)
        {
            SimpleMovement.MoveLissajous(ctx, entity.basis, brand);
        }
    }

    public static void EndPattern(AnimationContext ctx, Pattern pattern, Entity entity)
    {
        if (pattern == Pattern.Bouncy)
        {
            BouncyPattern.OnEnd(ctx, entity);
        }
    }
}
