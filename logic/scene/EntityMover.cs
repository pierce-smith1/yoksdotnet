using System;
using System.Collections;
using System.Collections.Generic;

namespace yoksdotnet.logic.scene;

public delegate void MoveFunction(Scene scene, SceneEntity entity, IEnumerable<SceneEntity> allEntities);

public partial class MoverState
{
    public enum OffscreenBehaviors
    {
        Wrap,
        Clamp,
    }

    public OffscreenBehaviors OffscreenBehavior { get; set; } = OffscreenBehaviors.Wrap;
}

public static class EntityMovers
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
    };

    public static void OffscreenMover(Scene scene, SceneEntity entity, IEnumerable<SceneEntity> allEntities)
    {
        var bounds = entity.GetBounds();

        var xInBounds = bounds.BotRight.X >= 0 && bounds.TopLeft.X <= scene.Width;
        var yInBounds = bounds.BotRight.Y >= 0 && bounds.TopLeft.Y <= scene.Height;

        if (yInBounds && xInBounds) {
            // Our sight is seen -
            // We are not offscreen!
            return;
        }

        switch (entity.MoverState.OffscreenBehavior)
        {
            case MoverState.OffscreenBehaviors.Wrap:
                var boundsWidth = bounds.BotRight.X - bounds.TopLeft.X;
                var boundsHeight = bounds.BotRight.Y - bounds.TopLeft.Y;

                if (!xInBounds)
                {
                    entity.Home.X += (scene.Width + boundsWidth) * (entity.Home.X < 0 ? 1 : -1);
                } 

                if (!yInBounds)
                {
                    entity.Home.Y += (scene.Height + boundsHeight) * (entity.Home.Y < 0 ? 1 : -1);
                }

                break;

            case MoverState.OffscreenBehaviors.Clamp:
                entity.Home.X = Math.Clamp(entity.Home.X, entity.Offset.X, scene.Width + entity.Offset.X);
                entity.Home.Y = Math.Clamp(entity.Home.Y, entity.Offset.Y, scene.Width + entity.Offset.Y);

                break;
        }
    }
}
