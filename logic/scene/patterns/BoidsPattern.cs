using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene.patterns;

public class Boid : EntityComponent
{
    public required double avoidRadius;
    public required double visionRadius;
    public required double visionTheta;
}

public class BoidsPattern
{
    public static void Start(AnimationContext ctx, Entity entity, Brand brand)
    {
        var boid = entity.EnsureHas<Boid>(() => new()
        {
            avoidRadius = 60.0,
            visionRadius = 120.0,
            visionTheta = Math.PI / 5.0,
        });

        var physics = entity.EnsureHas<Physics>(() => new()
        {
            velocity = Vector.RandomScaled(ctx.rng, 2.0, 2.0),
            mass = Interp.Linear(brand.value, 0.0, 1.0, 0.5, 1.5),
        });
    }

    public static void Move(AnimationContext ctx, Entity entity, Brand brand)
    {
        var boid = entity.Get<Boid>()!;
        var physics = entity.Get<Physics>()!;

        var peersToAvoid = ctx.scene.entities.Where(e => ShouldAvoid(entity.basis, boid, e.basis));
        PushAwayFrom(entity.basis, physics, peersToAvoid.Select(e => e.basis));

        var visiblePeers = ctx.scene.entities.Where(e => IsVisibleTo(entity.basis, physics, boid, e.basis));
        AlignWith(entity.basis, physics, visiblePeers.Select(e => e.Get<Physics>()!));
        SteerTowardsCenter(entity.basis, physics, visiblePeers.Select(e => e.basis));

        ClampSpeed(physics);
        TurnFromEdges(ctx, entity.basis, physics);

        SpriteMovement.SimulatePhysics(ctx, entity.basis, physics);
        SpriteMovement.WrapScreen(ctx, entity.basis);
    }

    private static bool ShouldAvoid(Basis basis, Boid boid, Basis other)
    {
        var dist = basis.Final.DistanceTo(other.Final);
        return dist < boid.avoidRadius;
    }

    private static bool IsVisibleTo(Basis basis, Physics physics, Boid boid, Basis other)
    {
        var dist = basis.Final.DistanceTo(other.Final);

        var isInRadius = dist > boid.avoidRadius && dist < boid.visionRadius;

        var offset = other.Final.Sub(basis.Final);
        var dot = physics.velocity.AsNormalized().Dot(offset.AsNormalized());
        var isInFov = dot > Math.Cos(boid.visionTheta);

        return isInRadius && isInFov;
    }
    
    private static void PushAwayFrom(Basis basis, Physics physics, IEnumerable<Basis> others)
    {
        const double avoidanceStrength = 1.0 / 3.0;

        var pushForce = new Vector(0.0, 0.0);

        foreach (var other in others)
        {
            var dist = Math.Max(other.Final.DistanceTo(basis.Final), 0.5);

            pushForce.X += (basis.Final.X - other.Final.X) / dist;
            pushForce.Y += (basis.Final.Y - other.Final.Y) / dist;
        }

        pushForce.Mult(1.0 / others.Count());
        pushForce.Mult(avoidanceStrength);

        physics.velocity.X += pushForce.X;
        physics.velocity.Y += pushForce.Y;
    }

    private static void AlignWith(Basis basis, Physics physics, IEnumerable<Physics> others)
    {
        if (!others.Any())
        {
            return;
        }

        const double alignmentStrength = 1.0 / 10.0;

        var desiredVelocity = new Vector(0.0, 0.0);

        foreach (var other in others)
        {
            desiredVelocity.X += other.velocity.X;
            desiredVelocity.Y += other.velocity.Y;
        }

        desiredVelocity.Mult(1.0 / others.Count());

        physics.velocity.X += (desiredVelocity.X - physics.velocity.X) * alignmentStrength;
        physics.velocity.Y += (desiredVelocity.Y - physics.velocity.Y) * alignmentStrength;
    }

    private static void SteerTowardsCenter(Basis basis, Physics physics, IEnumerable<Basis> others)
    {
        if (!others.Any())
        {
            return;
        }

        const double centeringStrength = 1.0 / 2000.0;

        var centerOfMass = new Vector(0.0, 0.0);
        foreach (var other in others)
        {
            centerOfMass.X += other.Final.X;
            centerOfMass.Y += other.Final.Y;
        }

        centerOfMass.Mult(1.0 / others.Count());

        physics.velocity.X += (centerOfMass.X - basis.Final.X) * centeringStrength;
        physics.velocity.Y += (centerOfMass.Y - basis.Final.Y) * centeringStrength;
    }

    private static void TurnFromEdges(AnimationContext ctx, Basis basis, Physics physics)
    {
        const double margin = 100.0;
        const double turnStrength = 1.0 / 20.0;

        if (basis.Final.X < margin)
        {
            physics.velocity.X += turnStrength;
        }

        if (basis.Final.Y < margin)
        {
            physics.velocity.Y += turnStrength;
        }

        if (basis.Final.X > ctx.scene.width - margin)
        {
            physics.velocity.X -= turnStrength;
        }

        if (basis.Final.Y > ctx.scene.height - margin)
        {
            physics.velocity.Y -= turnStrength;
        }
    }

    private static void ClampSpeed(Physics physics)
    {
        const double maxSpeed = 2.5;

        if (physics.velocity.Magnitude > maxSpeed)
        {
            physics.velocity.Mult(maxSpeed / physics.velocity.Magnitude);
        }
    }
}
