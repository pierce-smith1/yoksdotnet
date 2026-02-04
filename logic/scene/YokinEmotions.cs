using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public static class YokinEmotions
{
    public static void UpdateEmotions(AnimationContext ctx)
    {
        var noiseFactor = Interp.Linear(ctx.options.emotionScale, 0.0, 1.0, 0.0, 2.0);

        foreach (var sprite in ctx.scene.sprites)
        {
            if (sprite.addons.emotions is not { } emotions)
            {
                continue;
            }

            emotions.ambition = GetNoiseForSprite(ctx, sprite, 0.0) * noiseFactor;
            emotions.empathy = GetNoiseForSprite(ctx, sprite, 1000.0) * noiseFactor;
            emotions.optimism = GetNoiseForSprite(ctx, sprite, 2000.0) * noiseFactor;
        }
    }

    private static double GetNoiseForSprite(AnimationContext ctx, Sprite sprite, double zOffset)
    {
        var noise = ctx.noiseGenerator.Get
        (
            sprite.home.X / 1000.0, 
            sprite.home.Y / 1000.0, 
            (ctx.scene.seconds + zOffset) / 50.0
        );

        var result = Interp.Linear(noise, 0.0, 1.0, -1.0, 1.0);
        return result;
    }
}
