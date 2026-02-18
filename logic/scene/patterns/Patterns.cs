using System;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene.patterns;

public record Pattern(
    string Name, 
    string Description, 
    Action<AnimationContext, Entity, Brand> MoveAction,
    Action<AnimationContext, Entity, Brand> StartAction
) 
    : ISfEnum
{
    public readonly static Pattern Lattice = new(
        "Lattice", 
        "No motion",
        MoveAction: SimpleMovement.Noop,
        StartAction: SimpleMovement.Noop
    );

    public readonly static Pattern Roamers = new(
        "Roamers", 
        "Drift from left to right, softly bobbing up and down",
        MoveAction: SimpleMovement.MoveRoamers,
        StartAction: SimpleMovement.Noop
    );

    public readonly static Pattern Bouncy = new(
        "Bouncy",
        "DVD screensaver style",
        MoveAction: BouncyPattern.Move,
        StartAction: BouncyPattern.Start
    );

    public readonly static Pattern Waves = new(
        "Waves", 
        "Move in large, random circles",
        MoveAction: SimpleMovement.MoveWaves,
        StartAction: SimpleMovement.Noop
    );

    public readonly static Pattern Lissajous = new(
        "Lissajous",
        "Line up and march in a snaking curve",
        MoveAction: SimpleMovement.MoveLissajous,
        StartAction: SimpleMovement.Noop
    );

    public readonly static Pattern Bubbles = new(
        "Bubbles",
        "Become bubbles and bounce off one another",
        MoveAction: BubblesPattern.Move,
        StartAction: BubblesPattern.Start
    );

    public override string ToString() => Name;
}
