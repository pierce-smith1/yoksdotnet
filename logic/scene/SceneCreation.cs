using System;
using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public static class SceneCreation
{
    public static Scene NewScene(ScrOptions options, Random rng, int width, int height)
    {
        var scene = new Scene
        {
            width = width,
            height = height,
            sprites = [..new SpriteGenerator(rng, options).Make(width, height)],
            currentPattern = options.startingPattern.Match(
                whenRandom: () => 
                    rng.SampleOrDefault(options.possiblePatterns) ?? Pattern.Lattice,
                whenSingle: pattern => pattern
            ),
        };

        return scene;
    }
}
