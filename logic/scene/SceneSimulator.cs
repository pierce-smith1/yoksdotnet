using System;
using yoksdotnet.common;
using yoksdotnet.data;

namespace yoksdotnet.logic.scene;

public static class SceneSimulation
{
    public static void HandleFrame(AnimationContext ctx)
    {
        var now = DateTimeOffset.Now;
        
        if (ctx.scene.lastTick is { } lastTick)
        {
            var dt = now - lastTick;

            var speedScale = Interp.Square(ctx.options.animationSpeed, 0.0, 1.0, 0.01, 0.5);

            ctx.scene.lastDtMs = dt.TotalMilliseconds * speedScale;
            ctx.scene.seconds += dt.TotalSeconds * speedScale;
        }

        ctx.scene.lastTick = now;
        ctx.scene.frame++;

        if (ctx.scene.frame == 1)
        {
            SpriteChoreography.HandleSceneStart(ctx);
        }

        SpriteChoreography.HandleFrame(ctx);
    }
}
