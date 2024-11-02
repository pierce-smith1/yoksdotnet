using System;

namespace yoksdotnet.logic.scene.patterns;

public enum PatternId
{
    Lattice,
    Roamers,
    Waves,
}

public static class Patterns
{
    public readonly static LatticePattern Lattice = new();
    public readonly static RoamersPattern Roamers = new();
    public readonly static WavesPattern Waves = new();

    public static MoveFunction GetFunction(PatternId patternId)
    {
        PatternDefinition patternDefinition = patternId switch
        {
            PatternId.Lattice => Lattice,
            PatternId.Roamers => Roamers,
            PatternId.Waves => Waves,

            _ => throw new NotImplementedException(),
        };

        return patternDefinition.GetMoveFunction();
    }
}

public abstract class PatternDefinition
{
    abstract public MoverState.OffscreenBehaviors GetOffscreenBehavior();
    abstract protected MoveFunction GetBaseMoveFunction();

    public MoveFunction GetMoveFunction() => (scene, sprite, allSprites) =>
    {
        sprite.MoverState.OffscreenBehavior = GetOffscreenBehavior();
        GetBaseMoveFunction()(scene, sprite, allSprites);
    };
}

