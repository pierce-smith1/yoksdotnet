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
        yokin.Emotion.Empathy = GetNoiseForSprite(sprite, 1000.0);
        yokin.Emotion.Optimism = GetNoiseForSprite(sprite, 2000.0);
    }

    private double GetNoiseForSprite(Sprite sprite, double zOffset)
    {
        var noise = _perlinNoiseGenerator.Get
        (
            sprite.Home.X / 1000.0, 
            sprite.Home.Y / 1000.0, 
            (Scene.Seconds + zOffset) / 50.0
        );
        return noise;
    }
}
