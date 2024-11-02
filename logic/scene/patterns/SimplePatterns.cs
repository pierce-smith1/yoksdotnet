using System;

using static yoksdotnet.logic.scene.MoverState;

namespace yoksdotnet.logic.scene.patterns;

public class LatticePattern : PatternDefinition
{
    public override OffscreenBehaviors GetOffscreenBehavior() => OffscreenBehaviors.Wrap;
    protected override MoveFunction GetBaseMoveFunction() => (scene, sprite, allSprites) => { };
}

public class RoamersPattern : PatternDefinition
{
    public override OffscreenBehaviors GetOffscreenBehavior() => OffscreenBehaviors.Wrap;
    protected override MoveFunction GetBaseMoveFunction() => (scene, sprite, allSprites) =>
    {
        sprite.Home.X += sprite.Brand + 0.2;
        sprite.Home.Y += Math.Sin(scene.Seconds * sprite.Brand);
    };
}

public class WavesPattern : PatternDefinition
{
    public override OffscreenBehaviors GetOffscreenBehavior() => OffscreenBehaviors.Wrap;
    protected override MoveFunction GetBaseMoveFunction() => (scene, sprite, allSprites) =>
    {
        sprite.Home.X += Math.Sin(scene.Seconds * sprite.Brand);
        sprite.Home.Y += Math.Cos(scene.Seconds * sprite.Brand);
    };
}
