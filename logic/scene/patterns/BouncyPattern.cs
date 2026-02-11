using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yoksdotnet.logic.scene.patterns;

public static class BouncyPattern
{
    public static void Move(AnimationContext ctx, Entity entity)
    {
        var velocity = entity.physics?.velocity
            ?? entity.physicsMeasurements?.lastVelocity
            ?? new(ctx.rng.NextDouble(), ctx.rng.NextDouble());

        entity.basis.home.X += velocity.X * ctx.scene.lastDtMs;
        entity.basis.home.Y += velocity.Y * ctx.scene.lastDtMs;

        if (entity.basis.home.X < 0)
        {
            entity.basis.home.X = 0;
            velocity.X = -velocity.X;
        }

        if (entity.basis.home.Y < 0)
        {
            entity.basis.home.Y = 0;
            velocity.Y = -velocity.Y;
        }

        if (entity.basis.home.X + (entity.basis.width * entity.basis.scale) > ctx.scene.width)
        {
            entity.basis.home.X = ctx.scene.width - (entity.basis.width * entity.basis.scale);
            velocity.X = -velocity.X;
        }

        if (entity.basis.home.Y + (entity.basis.height * entity.basis.scale) > ctx.scene.height)
        {
            entity.basis.home.Y = ctx.scene.height - (entity.basis.height * entity.basis.scale);
            velocity.Y = -velocity.Y;
        }

        entity.physics ??= new()
        {
            velocity = new(0.0, 0.0),
            acceleration = new(0.0, 0.0),
        };

        entity.physics.velocity = velocity;
    }

    public static void OnEnd(AnimationContext ctx, Entity entity)
    {
        entity.physics = null;
    }
}
