using System.Collections.Generic;

namespace yoksdotnet.logic.scene;

public class EmotionHandler(Scene scene, PerlinNoiseGenerator noiseGenerator)
{
    public void UpdateEmotions(IEnumerable<Yokin> yokins)
    {
        foreach (var yokin in yokins)
        {
            yokin.emotions.Ambition = GetNoiseForSprite(yokin, 0.0);
            yokin.emotions.Empathy = GetNoiseForSprite(yokin, 1000.0);
            yokin.emotions.Optimism = GetNoiseForSprite(yokin, 2000.0);
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
