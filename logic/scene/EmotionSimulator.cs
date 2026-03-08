using System;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene;

public static class EmotionSimulator
{
    public static void UpdateEmotions(AnimationContext ctx, (Basis, Emotion) entity)
    {
        var (basis, emotion) = entity;

        var noiseFactor = Interp.Sqrt(ctx.options.emotionScale, 0.0, 1.0, 0.0, 3.0);

        emotion.ambition = GetNoiseValue(ctx, basis, 0.0) * noiseFactor;
        emotion.empathy = GetNoiseValue(ctx, basis, 1000.0) * noiseFactor;
        emotion.optimism = GetNoiseValue(ctx, basis, 2000.0) * noiseFactor;
    }

    private static double GetNoiseValue(AnimationContext ctx, Basis basis, double zOffset)
    {
        var noise = ctx.noiseGenerator.Get
        (
            basis.home.X / 1000.0, 
            basis.home.Y / 1000.0, 
            (ctx.scene.seconds + zOffset) / 2.0
        );

        var result = Interp.Linear(noise, 0.0, 1.0, -1.0, 1.0);
        return result;
    }
}
