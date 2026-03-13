using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene.patterns;

public class BoidsSimulator : PatternSimulator
{
    private readonly static double _baseAvoidRadius = 60.0;
    private readonly static double _baseVisionRadius = 120.0;
    private readonly static double _baseVisionTheta = Math.PI / 5.0;
    private readonly static double _maxInteractionDistance = _baseVisionRadius;

    public override void Init(AnimationContext ctx)
    {
        foreach (var entity in ctx.scene.entities)
        {
            var boid = entity.boid ??= new()
            {
                avoidRadius = _baseAvoidRadius,
                visionRadius = _baseVisionRadius,
                visionTheta = _baseVisionTheta,
            };

            var physics = entity.physics ??= new()
            {
                velocity = Vector.RandomScaled(ctx.rng, 2.0, 2.0),
            };
        }

        ctx.scene.entityBlocks = EntityBlockMapper.InitBlocks(ctx.scene, _maxInteractionDistance);
    }

    public override void BeforeMove(AnimationContext ctx)
    {
        EntityBlockMapper.AssignEntities(ctx.scene, ctx.scene.entityBlocks, ctx.scene.entities, _maxInteractionDistance);
    }

    public override void MoveEntity(AnimationContext ctx, Entity entity)
    {
        var physics = entity.physics!;
        var boid = entity.boid!;

        var block = entity.block!;

        var peersToAvoid = entity.block!.interactibleEntities
            .Where(e => ShouldAvoid((entity.basis, boid), e.basis));

        PushAwayFrom((entity.basis, physics), peersToAvoid.Select(e => e.basis));

        var visiblePeers = entity.block!.interactibleEntities
            .Where(e => IsVisibleTo((entity.basis, physics, boid), e.basis));

        AlignWith((entity.basis, physics), visiblePeers.Select(e => e.physics!));
        SteerTowardsCenter((entity.basis, physics), visiblePeers.Select(e => e.basis));

        ClampSpeed(physics);
        TurnFromEdges(ctx, (entity.basis, physics));

        SpriteMovement.SimulatePhysics(ctx, (entity.basis, physics));
        SpriteMovement.WrapScreen(ctx, entity.basis);
    }

    private static bool ShouldAvoid((Basis, Boid) entity, Basis other)
    {
        var (basis, boid) = entity;

        var dist = basis.Final.DistanceTo(other.Final);
        return dist < boid.avoidRadius;
    }

    private static bool IsVisibleTo((Basis, Physics, Boid) entity, Basis other)
    {
        var (basis, physics, boid) = entity;

        var dist = basis.Final.DistanceTo(other.Final);

        var isInRadius = dist > boid.avoidRadius && dist < boid.visionRadius;

        var offset = other.Final.Sub(basis.Final);
        var dot = physics.velocity.AsNormalized().Dot(offset.AsNormalized());
        var isInFov = dot > Math.Cos(boid.visionTheta);

        return isInRadius && isInFov;
    }
    
    private static void PushAwayFrom((Basis, Physics) entity, IEnumerable<Basis> others)
    {
        var (basis, physics) = entity;

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

    private static void AlignWith((Basis, Physics) entity, IEnumerable<Physics> others)
    {
        var (basis, physics) = entity;

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

    private static void SteerTowardsCenter((Basis, Physics) entity, IEnumerable<Basis> others)
    {
        var (basis, physics) = entity;
    
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

    private static void TurnFromEdges(AnimationContext ctx, (Basis, Physics) entity)
    {
        var (basis, physics) = entity;

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
