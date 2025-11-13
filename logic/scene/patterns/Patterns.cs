using System;
using System.Text.Json.Serialization;

using yoksdotnet.common;

namespace yoksdotnet.logic.scene.patterns;

public class Pattern : IStaticFieldEnumeration
{
    public readonly static Pattern Lattice = new(
        "Lattice",
        MoverState.OffscreenBehaviors.Wrap,
        (scene, sprite, allSprites) => {}
    );

    public readonly static Pattern Roamers = new(
        "Roamers",
        MoverState.OffscreenBehaviors.Wrap,
        (scene, sprite, allSprites) =>
        {
            sprite.Home.X += sprite.Brand + 0.2;
            sprite.Home.Y += Math.Sin(scene.Seconds * sprite.Brand);
        }
    );

    public readonly static Pattern Waves = new(
        "Waves",
        MoverState.OffscreenBehaviors.Wrap,
        (scene, sprite, allSprites) =>
        {
            sprite.Home.X += Math.Sin(scene.Seconds * sprite.Brand);
            sprite.Home.Y += Math.Cos(scene.Seconds * sprite.Brand);
        }
    );

    public string Name { get; init; }
    public MoverState.OffscreenBehaviors OffscreenBehavior { get; init; }
    public MoveFunction Move { get; init; }

    private Pattern(
        string displayName, 
        MoverState.OffscreenBehaviors offScreenBehavior, 
        MoveFunction moveFn
    ) {
        Name = displayName;
        OffscreenBehavior = offScreenBehavior;
        Move = moveFn;
    }

    public override string ToString() => Name;
}

[JsonDerivedType(typeof(Random), nameof(Random))]
[JsonDerivedType(typeof(SinglePattern), nameof(SinglePattern))]
public record PatternChoice
{
    public record Random() : PatternChoice()
    {
        public override string ToString() => "Random";
    }

    public record SinglePattern(Pattern Pattern) : PatternChoice()
    {
        public override string ToString() => Pattern.Name;
    }
}
