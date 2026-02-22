using System;
using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene.patterns;

public class SimpleMovement
{
    public static void Noop(AnimationContext _ctx, Entity _entity, Brand _brand) {}

    public static void MoveRoamers(AnimationContext ctx, Entity entity, Brand brand)
    {
        entity.basis.home.X += brand.value * ctx.scene.lastDtMs;
        entity.basis.home.Y += Math.Sin(ctx.scene.seconds * brand.value) * ctx.scene.lastDtMs;

        SpriteMovement.WrapScreen(ctx, entity.basis);
    }
    public static void MoveWaves(AnimationContext ctx, Entity entity, Brand brand)
    {
        entity.basis.home.X += Math.Sin(ctx.scene.seconds * brand.value) * ctx.scene.lastDtMs;
        entity.basis.home.Y += Math.Cos(ctx.scene.seconds * brand.value) * ctx.scene.lastDtMs;

        SpriteMovement.WrapScreen(ctx, entity.basis);
    }
    public static void MoveLissajous(AnimationContext ctx, Entity entity, Brand brand)
    {
        var minX = Math.Max((ctx.scene.width - ctx.scene.height) / 2.0, 0.0);
        var maxX = ctx.scene.width - minX;

        var minY = Math.Max((ctx.scene.height - ctx.scene.width) / 2.0, 0.0);
        var maxY = ctx.scene.height - minY;

        var targetX = Interp.Linear(
            Math.Sin(ctx.scene.seconds * 4.0 - brand.value * 17.0),
            -1.0, 1.0,
            minX, maxX - ClassicBitmap.Size
        );

        var targetY = Interp.Linear(
            Math.Cos(ctx.scene.seconds * 4.0 - brand.value * 11.0),
            -1.0, 1.0,
            minY, maxY - ClassicBitmap.Size
        );

        entity.basis.home.X = targetX + (entity.basis.home.X - targetX) * 0.9;
        entity.basis.home.Y = targetY + (entity.basis.home.Y - targetY) * 0.9;
    }
}
