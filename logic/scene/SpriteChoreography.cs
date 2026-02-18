using System;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic.scene;

public static class SpriteChoreography
{
    private static Brand _genericBrand = new() 
    {
        value = 0.0,
    };

    public static void HandleSceneStart(AnimationContext ctx)
    {
        if (ctx.scene.currentPattern is { } firstPattern)
        {
            foreach (var entity in ctx.scene.entities)
            {
                firstPattern.StartAction(ctx, entity, entity.Get<Brand>() ?? _genericBrand);
            }
        }
    }

    public static void HandleFrame(AnimationContext ctx)
    {
        var (endingPattern, startingPattern) = CheckPatternChange(ctx);

        if (startingPattern is not null)
        {
            foreach (var entity in ctx.scene.entities)
            {
                startingPattern.StartAction(ctx, entity, entity.Get<Brand>() ?? _genericBrand);
            }
        }

        foreach (var entity in ctx.scene.entities)
        {
            MoveSprite(ctx, entity);
        }
    }

    public static (Pattern? OldPattern, Pattern? NewPattern) CheckPatternChange(AnimationContext ctx)
    {
        var patternChanged = false;
        var oldPattern = ctx.scene.currentPattern;

        if (ShouldChangePattern(ctx))
        {
            patternChanged = ChangePattern(ctx);
        }

        if (patternChanged && ctx.scene.currentPattern is not null)
        {
            return (oldPattern, ctx.scene.currentPattern);
        }

        return (null, null);
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

        currentPattern.MoveAction(ctx, entity, entity.Get<Brand>() ?? _genericBrand);

        if (entity.Get<Emotion>() is { } emotion)
        {
            SpriteMovement.ApplyEmotionShake(ctx, emotion, entity.basis);
            YokinEmotions.UpdateEmotions(ctx, emotion, entity.basis);
        }

        if (currentPattern != Pattern.Lattice)
        {
            var newHome = entity.basis.home;
            var measuredVelocity = new Vector(
                (newHome.X - oldHome.X) / ctx.scene.lastDtMs, 
                (newHome.Y - oldHome.Y) / ctx.scene.lastDtMs
            );

            var physicsMeasurements = entity.EnsureHas<PhysicsMeasurements>(() => new()
            {
                lastVelocity = new(0.0, 0.0),
            });

            physicsMeasurements.lastVelocity = measuredVelocity;
        }

        SpriteTrails.UpdateTrails(ctx, entity);
    }
}