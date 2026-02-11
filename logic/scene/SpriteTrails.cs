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
        if (entity.trail is null)
        {
            return;
        }

        var cycledSnapshot = entity.trail.snapshots.First() ?? CreatureCreation.NewDefault();
        RefreshSnapshot(cycledSnapshot, entity);
        entity.trail.snapshots.Last() = cycledSnapshot;

        entity.trail.snapshots.Shift();
    }

    private static void RefreshSnapshot(Entity ghost, Entity entity)
    {
        ghost.basis.home = entity.basis.home;
        ghost.basis.offset = entity.basis.offset;
        ghost.basis.scale = entity.basis.scale;
        ghost.basis.width = entity.basis.width;
        ghost.basis.height = entity.basis.height;
        ghost.basis.angleRadians = entity.basis.angleRadians;

        if (entity.skin is null)
        {
            return;
        }

        ghost.skin ??= new()
        {
            palette = Palette.DefaultPalette,
        };
        
        ghost.skin.palette = entity.skin.palette;
        ghost.skin.fixedBitmap = entity.skin.fixedBitmap;
        ghost.skin.cachedPaint = entity.skin.cachedPaint;

        ghost.emotion = entity.emotion;
    }
}
