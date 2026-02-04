using System.Collections.Generic;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public class EmotionHandler(Scene scene, ScrOptions options, PerlinNoiseGenerator noiseGenerator)
{
    public void UpdateEmotions(IEnumerable<Sprite> sprites)
    {
        foreach (var sprite in sprites)
        {
            if (sprite.addons.emotions is not { } emotions)
            {
                continue;
            }

            emotions.ambition = GetNoiseForSprite(sprite, 0.0) * GetNoiseFactor();
            emotions.empathy = GetNoiseForSprite(sprite, 1000.0) * GetNoiseFactor();
            emotions.optimism = GetNoiseForSprite(sprite, 2000.0) * GetNoiseFactor();
        }
    }

    private double GetNoiseForSprite(Sprite sprite, double zOffset)
    {
        var noise = noiseGenerator.Get
        (
            sprite.home.X / 1000.0, 
            sprite.home.Y / 1000.0, 
            (scene.seconds + zOffset) / 50.0
        );

        var result = Interp.Linear(noise, 0.0, 1.0, -1.0, 1.0);
        return result;
    }

    private double GetNoiseFactor()
    {
        var factor = Interp.Linear(options.IndividualEmotionScale, 0.0, 1.0, 0.0, 2.0);
        return factor;
    }
}
