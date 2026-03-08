using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene;

public static class TrailSimulator
{
    public static void UpdateTrails(AnimationContext ctx, Entity entity)
    {
        if (entity.trail is not { } trail)
        {
            return;
        }

        var cycledSnapshot = trail.snapshots.First() ?? CreatureCreation.NewDefault();
        RefreshSnapshot(cycledSnapshot, entity);
        trail.snapshots.Last() = cycledSnapshot;

        trail.snapshots.Shift();
    }

    private static void RefreshSnapshot(Entity ghost, Entity entity)
    {
        ghost.basis.home = entity.basis.home;
        ghost.basis.offset = entity.basis.offset;
        ghost.basis.scale = entity.basis.scale;
        ghost.basis.width = entity.basis.width;
        ghost.basis.height = entity.basis.height;
        ghost.basis.angleRadians = entity.basis.angleRadians;

        if (entity.skin is { } skin)
        {
            var ghostSkin = ghost.skin ??= new()
            {
                style = skin.style,
                palette = skin.palette,
            };

            ghostSkin.style = skin.style;
            ghostSkin.palette = skin.palette;
            ghostSkin.fixedBitmap = skin.fixedBitmap;
            ghostSkin.paintCache = skin.paintCache;
            ghostSkin.bodyPaintHandle = skin.bodyPaintHandle;
            ghostSkin.eyePaintHandle = skin.eyePaintHandle;
            ghostSkin.whitePaintHandle = skin.whitePaintHandle;
        }

        if (entity.gaze is { } gaze)
        {
            var ghostGaze = ghost.gaze ??= new()
            {
                currentGazePoint = gaze.currentGazePoint,
                targetGazePoint = gaze.targetGazePoint,
                targetChangeCooldownSeconds = gaze.targetChangeCooldownSeconds,
            };

            ghostGaze.currentGazePoint = gaze.currentGazePoint;
            ghostGaze.targetGazePoint = gaze.targetGazePoint;
            ghostGaze.lastTargetChange = gaze.lastTargetChange;
            ghostGaze.targetEntity = gaze.targetEntity;
            ghostGaze.targetChangeCooldownSeconds = gaze.targetChangeCooldownSeconds;
        }

        if (entity.emotion is { } emotion)
        {
            ghost.emotion = emotion;
        }
    }
}
