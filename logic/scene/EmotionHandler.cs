using System.Collections.Generic;

namespace yoksdotnet.logic.scene;

public class EmotionHandler(Scene scene, PerlinNoiseGenerator noiseGenerator)
{
    public void UpdateEmotions(IEnumerable<Sprite> sprites)
    {
        foreach (var sprite in sprites)
        {
            if (sprite.addons.emotions is not { } emotions)
            {
                continue;
            }

            emotions.ambition = GetNoiseForSprite(sprite, 0.0);
            emotions.empathy = GetNoiseForSprite(sprite, 1000.0);
            emotions.optimism = GetNoiseForSprite(sprite, 2000.0);
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
        var result = (noise * 2) - 1;
        return result;
    }
}
