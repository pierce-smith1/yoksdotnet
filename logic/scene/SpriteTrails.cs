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
                palette = Palette.DefaultPalette,
            });
            
            ghostSkin.palette = skin.palette;
            ghostSkin.fixedBitmap = skin.fixedBitmap;
            ghostSkin.cachedPaint = skin.cachedPaint;
        }

        if (entity.Get<Emotion>() is { } emotion)
        {
            ghost.Attach(emotion);
        }
    }
}
