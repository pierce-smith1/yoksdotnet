namespace yoksdotnet.logic.scene;

public class EmotionHandler
{
    public required Scene Scene { get; init; }
    public required ScrOptions Options { get; init; }

    private readonly PerlinNoiseGenerator _perlinNoiseGenerator = new();

    public void UpdateEmotions(Sprite sprite)
    {
        if (sprite is not Yokin yokin)
        {
            return;
        }

        yokin.Emotion.Ambition = GetNoiseForSprite(sprite, 0.0);
        yokin.Emotion.Empathy = GetNoiseForSprite(sprite, 100.0);
        yokin.Emotion.Optimism = GetNoiseForSprite(sprite, 200.0);
    }

    private double GetNoiseForSprite(Sprite sprite, double zOffset)
    {
        var noise = _perlinNoiseGenerator.Get(sprite.Home.X / 100.0, sprite.Home.Y / 100.0, Scene.Seconds + zOffset);
        return noise;
    }
}
