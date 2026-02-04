using System;
using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public static class SceneCreation
{
    public static Scene NewScene(ScrOptions options, Random rng, int width, int height)
    {
        var spriteGenerator = new SpriteGenerator(options, rng);
        var sampler = new RandomSampler(rng);

        var scene = new Scene
        {
            width = width,
            height = height,
            sprites = [..spriteGenerator.Make(width, height)],
            currentPattern = options.AnimationStartingPattern switch
            {
                RandomPatternChoice => 
                    sampler.SampleOrDefault(options.AnimationPossiblePatterns) ?? Pattern.Lattice,

                SinglePatternChoice(var pattern) => pattern,

                _ => throw new NotImplementedException()
            },
        };

        return scene;
    }
}
