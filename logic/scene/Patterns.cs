using System;
using System.Collections.Generic;

namespace yoksdotnet.logic.scene;

public enum PatternId
{
    Lattice,
    Roamers,
    Waves,
}

public static class Patterns
{
    public readonly static Dictionary<PatternId, MoveFunction> Functions = new()
    {
        { PatternId.Lattice, (scene, entity, allEntities) => {} },
        { PatternId.Roamers, (scene, entity, allEntities) =>
        {
            entity.MoverState.OffscreenBehavior = MoverState.OffscreenBehaviors.Wrap;

            entity.Home.X += entity.Brand + 0.2;
            entity.Home.Y += Math.Sin(scene.Seconds * entity.Brand);
        }},
        { PatternId.Waves, (scene, entity, allEntities) =>
        {
            entity.Home.X += Math.Sin(scene.Seconds * entity.Brand);
            entity.Home.Y += Math.Cos(scene.Seconds * entity.Brand);
        }},
    };
}

