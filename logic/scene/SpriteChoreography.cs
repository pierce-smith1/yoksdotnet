using System;
using System.Linq;

using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public static class SpriteChoreography
{
    public static void HandleFrame(AnimationContext ctx)
    {
        if (ShouldChangePattern(ctx))
        {
            ChangePattern(ctx);
        }

        SpriteMovement.MoveByCurrentPattern(ctx);
        SpriteMovement.ApplyEmotionShake(ctx);
        YokinEmotions.UpdateEmotions(ctx);
    }

    public static bool ShouldChangePattern(AnimationContext ctx)
    {
        if (!ctx.options.patternDoesChange)
        {
            return false;
        }

        ctx.scene.patternLastChangedAt ??= DateTimeOffset.Now;

        var patternChangeSeconds = Interp.Linear(ctx.options.patternChangeFrequency, 0.0, 1.0, 90.0, 5.0);

        var shouldChange = DateTimeOffset.Now > ctx.scene.patternLastChangedAt?.AddSeconds(patternChangeSeconds);
        return shouldChange;
    }

    public static void ChangePattern(AnimationContext ctx)
    {
        var possiblePatterns = ctx.options.possiblePatterns
            .Where(pattern => pattern != ctx.scene.currentPattern);

        if (!possiblePatterns.Any())
        {
            return;
        }

        ctx.scene.currentPattern = ctx.rng.Sample(possiblePatterns);
        ctx.scene.patternLastChangedAt = DateTimeOffset.Now;
    }
}