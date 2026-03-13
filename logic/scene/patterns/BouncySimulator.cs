using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene.patterns;

public class BouncySimulator : PatternSimulator
{
    public override void Init(AnimationContext ctx)
    {
        foreach (var entity in ctx.scene.entities)
        {
            var lastVelocity = entity.physicsMeasurements?.lastVelocity;

            var physics = entity.physics ??= new()
            {
                velocity = lastVelocity ?? Vector.RandomScaled(ctx.rng, 5.0, 5.0),
            };
        }
    }

    public override void MoveEntity(AnimationContext ctx, Entity entity)
    {
        SpriteMovement.SimulatePhysics(ctx, (entity.basis, entity.physics!));
        BounceOffScreen(ctx, (entity.basis, entity.physics!));
    }

    public static void BounceOffScreen(AnimationContext ctx, (Basis, Physics) entity)
    {
        var (basis, physics) = entity;

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
