using System;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public class Emotion : EntityComponent
{
    public required double ambition;
    public required double empathy;
    public required double optimism;

    public double Magnitude => Math.Sqrt(ambition * ambition + empathy * empathy + optimism * optimism);
}

public static class YokinEmotions
{
    public static void UpdateEmotions(AnimationContext ctx, Emotion emotion, Basis basis)
    {
        var noiseFactor = Interp.Linear(ctx.options.emotionScale, 0.0, 1.0, 0.0, 2.0);

        emotion.ambition = GetNoiseValue(ctx, basis, 0.0) * noiseFactor;
        emotion.empathy = GetNoiseValue(ctx, basis, 1000.0) * noiseFactor;
        emotion.optimism = GetNoiseValue(ctx, basis, 2000.0) * noiseFactor;
    }

    private static double GetNoiseValue(AnimationContext ctx, Basis physical, double zOffset)
    {
        var noise = ctx.noiseGenerator.Get
        (
            physical.home.X / 1000.0, 
            physical.home.Y / 1000.0, 
            (ctx.scene.seconds + zOffset) / 50.0
        );

        var result = Interp.Linear(noise, 0.0, 1.0, -1.0, 1.0);
        return result;
    }
}
