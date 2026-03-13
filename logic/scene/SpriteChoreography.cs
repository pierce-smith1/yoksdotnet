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
        ctx.scene.currentPattern.Simulator.Init(ctx);
    }

    public static void HandleFrame(AnimationContext ctx)
    {
        var (endingPattern, startingPattern) = TryPatternChange(ctx);

        endingPattern?.Simulator.End(ctx);
        startingPattern?.Simulator.Init(ctx);

        ctx.scene.currentPattern.Simulator.Run(ctx);
    }

    public static (Pattern? OldPattern, Pattern? NewPattern) TryPatternChange(AnimationContext ctx)
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
}