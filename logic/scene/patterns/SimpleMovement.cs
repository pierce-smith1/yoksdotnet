using System;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene.patterns;

public static class SimpleMovement
{
    public class LatticeSimulator : SimplePatternSimulator
    {
        public override void Move(AnimationContext _ctx, Entity _entity) {}
    }

    public class RoamerSimulator : SimplePatternSimulator
    {
        public override void Move(AnimationContext ctx, Entity entity)
        {
            entity.basis.home.X += entity.brand * ctx.scene.lastDtMs;
            entity.basis.home.Y += Math.Sin(ctx.scene.seconds * entity.brand) * ctx.scene.lastDtMs;

            SpriteMovement.WrapScreen(ctx, entity.basis);
        }
    }

    public class WavesSimulator : SimplePatternSimulator
    {
        public override void Move(AnimationContext ctx, Entity entity)
        {
            entity.basis.home.X += Math.Sin(ctx.scene.seconds * entity.brand) * ctx.scene.lastDtMs;
            entity.basis.home.Y += Math.Cos(ctx.scene.seconds * entity.brand) * ctx.scene.lastDtMs;

            SpriteMovement.WrapScreen(ctx, entity.basis);
        }
    }

    public class LissajousSimulator : SimplePatternSimulator
    {
        public override void Move(AnimationContext ctx, Entity entity)
        {
            var minX = Math.Max((ctx.scene.width - ctx.scene.height) / 2.0, 0.0);
            var maxX = ctx.scene.width - minX;

            var minY = Math.Max((ctx.scene.height - ctx.scene.width) / 2.0, 0.0);
            var maxY = ctx.scene.height - minY;

            var targetX = Interp.Linear(
                Math.Sin(ctx.scene.seconds * 4.0 - entity.brand * 17.0),
                -1.0, 1.0,
                minX, maxX - ClassicBitmap.Size
            );

            var targetY = Interp.Linear(
                Math.Cos(ctx.scene.seconds * 4.0 - entity.brand * 11.0),
                -1.0, 1.0,
                minY, maxY - ClassicBitmap.Size
            );

            entity.basis.home.X = targetX + (entity.basis.home.X - targetX) * 0.9;
            entity.basis.home.Y = targetY + (entity.basis.home.Y - targetY) * 0.9;
        }
    }
}
