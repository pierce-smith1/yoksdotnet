using System;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public static class SceneSimulation
{
    public static void HandleFrame(AnimationContext ctx)
    {
        var now = DateTimeOffset.Now;
        
        if (ctx.scene.lastTick is { } lastTick)
        {
            var dt = now - lastTick;

            var speedScale = Interp.Square(ctx.options.animationSpeed, 0.0, 1.0, 0.05, 0.5);

            ctx.scene.lastDtMs = dt.TotalMilliseconds * speedScale;
            ctx.scene.seconds += dt.TotalSeconds * speedScale;
        }

        ctx.scene.lastTick = now;
        ctx.scene.frame++;

        SpriteChoreography.HandleFrame(ctx);
    }
}
