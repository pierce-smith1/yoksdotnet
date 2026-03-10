using System;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene;

public static class SpriteChoreography
{
    public static void HandleSceneStart(AnimationContext ctx)
    {
        EntityBlockMapper.InitBlocks(ctx.scene);

        foreach (var entity in ctx.scene.entities)
        {
            entity.patternToken = ctx.scene.currentPattern.Simulator.InitSimulation(ctx, entity);
        }
    }

    public static void HandleFrame(AnimationContext ctx)
    {
        EntityBlockMapper.AssignEntities(ctx.scene, ctx.scene.entityBlocks, ctx.scene.entities);

        var (endingPattern, startingPattern) = CheckPatternChange(ctx);

        if (endingPattern is not null)
        {
            foreach (var entity in ctx.scene.entities)
            {
                entity.patternToken ??= endingPattern.Simulator.InitSimulation(ctx, entity);
                endingPattern.Simulator.EndSimulation(ctx, entity, entity.patternToken);
            }
        }

        if (startingPattern is not null)
        {
            foreach (var entity in ctx.scene.entities)
            {
                entity.patternToken = startingPattern.Simulator.InitSimulation(ctx, entity);
            }
        }

        foreach (var entity in ctx.scene.entities)
        {
            MoveSprite(ctx, entity);

            if (entity.skin?.style.IsRefined is true)
            {
                GazeSimulator.UpdateGaze(ctx, entity);
            }
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

        entity.patternToken ??= currentPattern.Simulator.InitSimulation(ctx, entity);
        currentPattern.Simulator.Simulate(ctx, entity, entity.patternToken);

        if (entity.emotion is { } emotion)
        {
            SpriteMovement.ApplyEmotionShake(ctx, (entity.basis, emotion));
            EmotionSimulator.UpdateEmotions(ctx, (entity.basis, emotion));
        }

        if (currentPattern != Pattern.Lattice)
        {
            var newHome = entity.basis.home;
            var measuredVelocity = new Vector(
                (newHome.X - oldHome.X) / ctx.scene.lastDtMs, 
                (newHome.Y - oldHome.Y) / ctx.scene.lastDtMs
            );

            var physicsMeasurements = entity.physicsMeasurements ??= new()
            {
                lastVelocity = new(0.0, 0.0),
            };

            physicsMeasurements.lastVelocity = measuredVelocity;
        }

        TrailSimulator.UpdateTrails(ctx, entity);
    }
}