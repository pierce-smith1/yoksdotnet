using System;
using System.Linq;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene.patterns;

public static class SimpleMovement
{
    public class LatticeSimulator : PatternSimulator
    {
        public override void MoveEntity(AnimationContext _ctx, Entity entity) {}
    }

    public class RoamersSimulator : PatternSimulator
    {
        public override void MoveEntity(AnimationContext ctx, Entity entity)
        {
            entity.basis.home.X += entity.brand * ctx.scene.lastDtMs;
            entity.basis.home.Y += Math.Sin(ctx.scene.seconds * entity.brand) * ctx.scene.lastDtMs;

            SpriteMovement.WrapScreen(ctx, entity.basis);
        }
    }

    public class WavesSimulator : PatternSimulator
    {
        public override void MoveEntity(AnimationContext ctx, Entity entity)
        {
            entity.basis.home.X += Math.Sin(ctx.scene.seconds * entity.brand) * ctx.scene.lastDtMs;
            entity.basis.home.Y += Math.Cos(ctx.scene.seconds * entity.brand) * ctx.scene.lastDtMs;

            SpriteMovement.WrapScreen(ctx, entity.basis);
        }
    }

    public class LissajousSimulator : PatternSimulator
    {
        public override void MoveEntity(AnimationContext ctx, Entity entity)
        {
            var bounds = entity.basis.Bounds;
            var bufferX = (bounds.bottomRight.X - bounds.topLeft.X);
            var bufferY = (bounds.bottomRight.Y - bounds.topLeft.Y);

            var brandX = ctx.scene.entities.FirstOrDefault()?.brand ?? 0.5;
            var brandY = ctx.scene.entities.LastOrDefault()?.brand ?? 0.5;

            var minX = Math.Max((ctx.scene.width - ctx.scene.height) / 2.0, 0.0) + bufferX;
            var maxX = ctx.scene.width - minX - bufferX;

            var minY = Math.Max((ctx.scene.height - ctx.scene.width) / 2.0, 0.0) + bufferY;
            var maxY = ctx.scene.height - minY - bufferY;

            var coeffX = Interp.Linear(brandX, 0.0, 1.0, 10.0, 20.0);
            var coeffY = Interp.Linear(brandY, 0.0, 1.0, 10.0, 20.0);

            var targetX = Interp.Linear(
                Math.Sin(ctx.scene.seconds * 5.0 - entity.brand * coeffX),
                -1.0, 1.0,
                minX, maxX
            );

            var targetY = Interp.Linear(
                Math.Cos(ctx.scene.seconds * 5.0 - entity.brand * coeffY),
                -1.0, 1.0,
                minY, maxY
            );

            entity.basis.home.X = targetX + (entity.basis.home.X - targetX) * 0.9;
            entity.basis.home.Y = targetY + (entity.basis.home.Y - targetY) * 0.9;
        }
    }
}
