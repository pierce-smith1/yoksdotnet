using System;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public static class SpriteChoreography
{
    public static void HandleFrame(AnimationContext ctx)
    {
        var endedPattern = CheckPatternChange(ctx);

        foreach (var entity in ctx.scene.entities)
        {
            MoveSprite(ctx, entity);

            if (endedPattern is not null)
            {
                PatternMovement.EndPattern(ctx, endedPattern, entity);
            }
        }
    }

    public static Pattern? CheckPatternChange(AnimationContext ctx)
    {
        var patternChanged = false;
        var oldPattern = ctx.scene.currentPattern;

        if (ShouldChangePattern(ctx))
        {
            patternChanged = ChangePattern(ctx);
        }

        return patternChanged ? oldPattern : null;
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

    public static bool ChangePattern(AnimationContext ctx)
    {
        var possiblePatterns = ctx.options.possiblePatterns
            .Where(pattern => pattern != ctx.scene.currentPattern);

        if (!possiblePatterns.Any())
        {
            return false;
        }

        ctx.scene.currentPattern = ctx.rng.Sample(possiblePatterns);
        ctx.scene.patternLastChangedAt = DateTimeOffset.Now;

        return true;
    }

    private static void MoveSprite(AnimationContext ctx, Entity entity)
    {
        var currentPattern = ctx.scene.currentPattern ?? Pattern.Lattice;

        var oldHome = entity.basis.home;

        PatternMovement.MoveByPattern(ctx, currentPattern, entity);

        if (entity.emotion is not null)
        {
            SpriteMovement.ApplyEmotionShake(ctx, entity.emotion, entity.basis);
            YokinEmotions.UpdateEmotions(ctx, entity.emotion, entity.basis);
        }

        if (currentPattern != Pattern.Lattice)
        {
            var newHome = entity.basis.home;
            var measuredVelocity = new Point(
                (newHome.X - oldHome.X) / ctx.scene.lastDtMs, 
                (newHome.Y - oldHome.Y) / ctx.scene.lastDtMs
            );

            entity.physicsMeasurements ??= new()
            {
                lastVelocity = new(),
            };

            entity.physicsMeasurements.lastVelocity = measuredVelocity;
        }

        SpriteTrails.UpdateTrails(ctx, entity);
    }
}