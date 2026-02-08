using System;
using System.Collections.Generic;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class Trail : EntityComponent
{
    public required DateTimeOffset lastCycleAt;
    public required List<Entity> snapshots;
}

public static class SpriteTrails
{
    public static void UpdateTrails(AnimationContext ctx, Entity entity)
    {
        if (entity.trail is null)
        {
            return;
        }

        var trailRefreshSeconds = 0.2;

        if (entity.trail.lastCycleAt.AddSeconds(trailRefreshSeconds) < DateTimeOffset.Now)
        {
            var cycledSnapshot = entity.trail.snapshots[0];
            entity.trail.snapshots.RemoveAt(0);

            RefreshSnapshot(cycledSnapshot, entity);
            entity.trail.snapshots.Add(cycledSnapshot);

            entity.trail.lastCycleAt = DateTimeOffset.Now;
        }
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
    }
}
