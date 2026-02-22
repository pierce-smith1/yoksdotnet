using System;
using System.Linq;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene;

public class Gaze : EntityComponent
{
    public required Vector currentGazePoint;
    public required Vector targetGazePoint;
    public required double targetChangeCooldownSeconds;
    public Entity? targetEntity;
    public DateTimeOffset? lastTargetChange;
}

public static class GazeSimulator
{
    public static void UpdateGaze(AnimationContext ctx, Entity entity)
    {
        var gaze = entity.EnsureHas<Gaze>(() => new()
        {
            currentGazePoint = new(0.0, 0.0),
            targetGazePoint = new(0.0, 0.0),
            targetChangeCooldownSeconds = Interp.Linear(entity.Get<Brand>()?.value ?? 0.5, 0.0, 1.0, 1.5, 3.0),
        });

        UpdateTarget(ctx, entity, gaze);
        UpdateCurrentGazePoint(ctx, entity, gaze);
    }

    private static void UpdateTarget(AnimationContext ctx, Entity entity, Gaze gaze)
    {
        gaze.lastTargetChange ??= DateTimeOffset.Now;

        var canChangeTarget = DateTimeOffset.Now >= gaze.lastTargetChange + TimeSpan.FromSeconds(gaze.targetChangeCooldownSeconds);
        if (canChangeTarget)
        {
            var closestPeer = ctx.scene.entities
                .Where(e => e != entity)
                .MinBy(e => e.basis.Final.DistanceTo(entity.basis.Final));

            if (closestPeer is not null)
            {
                gaze.targetEntity = closestPeer;
            }
            else
            {
                gaze.targetEntity = null;
                gaze.targetGazePoint = new(ctx.rng.NextDouble() * ctx.scene.width, ctx.rng.NextDouble() * ctx.scene.height); 
            }

            gaze.lastTargetChange = DateTimeOffset.Now;
        }

        if (gaze.targetEntity is not null)
        {
            gaze.targetGazePoint = gaze.targetEntity.basis.Final;
        }
    }

    private static void UpdateCurrentGazePoint(AnimationContext ctx, Entity entity, Gaze gaze)
    {
        gaze.currentGazePoint.X += (gaze.targetGazePoint.X - gaze.currentGazePoint.X) * 0.05;
        gaze.currentGazePoint.Y += (gaze.targetGazePoint.Y - gaze.currentGazePoint.Y) * 0.05;
    }
}
