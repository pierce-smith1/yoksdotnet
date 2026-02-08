using System;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public static class SpriteChoreography
{
    public static void HandleFrame(AnimationContext ctx)
    {
        if (ShouldChangePattern(ctx))
        {
            ChangePattern(ctx);
        }

        foreach (var entity in ctx.scene.entities)
        {
            var currentPattern = ctx.scene.currentPattern ?? Pattern.Lattice;

            PatternMovement.MoveByPattern(ctx, currentPattern, entity);

            if (entity.emotion is not null)
            {
                SpriteMovement.ApplyEmotionShake(ctx, entity.emotion, entity.basis);
                YokinEmotions.UpdateEmotions(ctx, entity.emotion, entity.basis);
            }

            SpriteTrails.UpdateTrails(ctx, entity);
        }
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