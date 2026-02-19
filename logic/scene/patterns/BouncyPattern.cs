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

    public static void BounceOffScreen(AnimationContext ctx, Basis basis, Physics physics)
    {
        var bounds = basis.Bounds;

        if (bounds.topLeft.X < 0)
        {
            physics.velocity.X = EnsurePositive(physics.velocity.X);
        }

        if (bounds.topLeft.Y < 0)
        {
            physics.velocity.Y = EnsurePositive(physics.velocity.Y);
        }

        if (bounds.bottomRight.X > ctx.scene.width)
        {
            physics.velocity.X = EnsureNegative(physics.velocity.X);
        }

        if (bounds.bottomRight.Y > ctx.scene.height)
        {
            physics.velocity.Y = EnsureNegative(physics.velocity.Y);
        }
    }

    private static double EnsurePositive(double val)
    {
        var result = val < 0 ? -val : val;
        return result;
    }

    private static double EnsureNegative(double val)
    {
        var result = val < 0 ? val : -val;
        return result;
    }
}
