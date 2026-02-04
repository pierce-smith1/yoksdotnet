using System;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public class SceneSimulator(Scene scene, ScrOptions options, Random rng)
{
    private readonly SpriteChoregrapher _choreographer = new(scene, options, rng);

    public void TickSimulation()
    {
        var now = DateTimeOffset.Now;
        
        if (scene.lastTick is { } lastTick)
        {
            var dt = now - lastTick;

            scene.lastDtMs = dt.TotalMilliseconds * DerivedSpeedScale();
            scene.seconds += dt.TotalSeconds * DerivedSpeedScale();
        }

        scene.lastTick = now;
        scene.frame++;

        _choreographer.HandleFrame();
    }

    private double DerivedSpeedScale()
    {
        var scale = Interp.Square(options.AnimationSpeed, 0.0, 1.0, 0.05, 0.5);
        return scale;
    }
}
