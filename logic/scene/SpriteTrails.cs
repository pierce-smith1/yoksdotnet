using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class Trail : EntityComponent
{
    public required CircularBuffer<Entity?> snapshots;
}

public static class SpriteTrails
{
    public static void UpdateTrails(AnimationContext ctx, Entity entity)
    {
        if (entity.Get<Trail>() is not { } trail)
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

        if (entity.Get<Skin>() is { } skin)
        {
            var ghostSkin = ghost.EnsureHas<Skin>(() => new()
            {
                style = skin.style,
                palette = skin.palette,
            });

            ghostSkin.style = skin.style;
            ghostSkin.palette = skin.palette;
            ghostSkin.fixedBitmap = skin.fixedBitmap;
            ghostSkin.paintCache = skin.paintCache;
            ghostSkin.bodyPaintHandle = skin.bodyPaintHandle;
            ghostSkin.eyePaintHandle = skin.eyePaintHandle;
            ghostSkin.whitePaintHandle = skin.whitePaintHandle;
        }

        if (entity.Get<Emotion>() is { } emotion)
        {
            ghost.Attach(emotion);
        }
    }
}
