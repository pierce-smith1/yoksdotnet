using System;

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

            scene.lastDtMs = dt.TotalMilliseconds * options.GetActualAnimationSpeedScale();
            scene.seconds += dt.TotalSeconds * options.GetActualAnimationSpeedScale();
        }

        scene.lastTick = now;
        scene.frame++;

        _choreographer.HandleFrame();
    }
}
