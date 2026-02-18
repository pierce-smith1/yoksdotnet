using yoksdotnet.common;

namespace yoksdotnet.logic.scene.patterns;

public static class BouncyPattern
{
    public static void Start(AnimationContext ctx, Entity entity, Brand _brand)
    {
        var lastVelocity = entity.Get<PhysicsMeasurements>()?.lastVelocity;

        entity.EnsureHas<Physics>(() => new()
        {
            velocity = lastVelocity ?? Vector.RandomScaled(ctx.rng, 5.0, 5.0),
            mass = Interp.Linear(ctx.rng.NextDouble(), 0.0, 1.0, 0.5, 1.5),
        });
    }

    public static void Move(AnimationContext ctx, Entity entity, Brand _brand)
    {
        if (entity.Get<Physics>() is not { } physics)
        {
            return;
        }

        SpriteMovement.SimulatePhysics(ctx, entity.basis, physics);
        BounceOffScreen(ctx, entity.basis, physics);
    }

    public static void BounceOffScreen(AnimationContext ctx, PhysicalBasis basis, Physics physics)
    {
        var bounds = basis.Bounds;

        if (bounds.topLeft.X < 0)
        {
            basis.home.X = basis.ApothemX;
            physics.velocity.X = -physics.velocity.X;
        }

        if (bounds.topLeft.Y < 0)
        {
            basis.home.Y = basis.ApothemY;
            physics.velocity.Y = -physics.velocity.Y;
        }

        if (bounds.bottomRight.X > ctx.scene.width)
        {
            basis.home.X = ctx.scene.width - basis.ApothemX;
            physics.velocity.X = -physics.velocity.X;
        }

        if (bounds.bottomRight.Y > ctx.scene.height)
        {
            basis.home.Y = ctx.scene.height - basis.ApothemY;
            physics.velocity.Y = -physics.velocity.Y;
        }
    }
}
