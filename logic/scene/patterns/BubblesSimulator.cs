using Serilog;
using System;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene.patterns;

public class BubblesSimulator : PatternSimulator
{
    private readonly static double _minBubbleSize = 2.0;
    private readonly static double _maxBubbleSize = 50.0;
    private readonly static double _minMass = 0.5;
    private readonly static double _maxMass = 1.5;
    private readonly static double _massSizeBoostFactor = 10.0;
    private readonly static double _maxInteractionDistance = (_maxBubbleSize + _maxMass * _massSizeBoostFactor) * 2.0;

    public override void Init(AnimationContext ctx)
    {
        foreach (var entity in ctx.scene.entities)
        {
            var physics = entity.physics ??= new()
            {
                velocity = Vector.RandomScaled(ctx.rng, 2.0, 2.0),
            };

            var mass = Interp.Linear(entity.brand, 0.0, 1.0, _minMass, _maxMass);

            var bubble = entity.bubble ??= new()
            {
                radius = Interp.Linear(ctx.options.spriteScale, 0.0, 1.0, _minBubbleSize, _maxBubbleSize) + mass * _massSizeBoostFactor,
                mass = mass,
            };

            bubble.isFree = false;
            bubble.SetVisible(true);
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
        var bubble = entity.bubble!;

        var isColliding = false;
        foreach (var peer in entity.block!.interactibleEntities)
        {
            if (peer == entity)
            {
                continue;
            }

            if (!AreColliding((entity.basis, bubble), (peer.basis, peer.bubble!)))
            {
                continue;
            }

            isColliding = true;
            var peerBubble = peer.bubble!;

            if (bubble.isFree && peerBubble.isFree)
            {
                SimulateCollision((entity.basis, physics, bubble), (peer.basis, peer.physics!, peerBubble));
                PushApart(entity.basis, bubble, peer.basis, peerBubble);
            }
        }

        if (!isColliding)
        {
            bubble.isFree = true;
        }

        ApplyDrift(ctx, physics, entity.brand);

        SpriteMovement.SimulatePhysics(ctx, (entity.basis, physics));
        BouncySimulator.BounceOffScreen(ctx, (entity.basis, physics));
    }

    public override void End(AnimationContext ctx)
    {
        foreach (var entity in ctx.scene.entities)
        {
            if (entity.bubble is not { } bubble)
            {
                continue;
            }

            bubble.SetVisible(false);
        }
    }

    private static void ApplyDrift(AnimationContext ctx, Physics physics, double brand)
    {
        var driftX = Math.Sin(ctx.scene.seconds * 2.0 + brand * 10.0) / 200.0;
        var driftY = Math.Cos(ctx.scene.seconds * 3.0 + brand * 10.0) / 200.0;

        physics.velocity.X += driftX;
        physics.velocity.Y += driftY;
    }

    private static bool AreColliding((Basis, Bubble) e1, (Basis, Bubble) e2)
    {
        var (basis1, bubble1) = e1;
        var (basis2, bubble2) = e2;

        var radiusDist = bubble1.radius + bubble2.radius;
        var isColliding = basis1.Final.DistanceTo(basis2.Final) <= radiusDist;
        return isColliding;
    }

    private static void SimulateCollision((Basis, Physics, Bubble) e1, (Basis, Physics, Bubble) e2)
    {
        const double energyRetention = 0.95;

        var (basis1, physics1, bubble1) = e1;
        var (basis2, physics2, bubble2) = e2;

        var v1 = physics1.velocity;
        var v2 = physics2.velocity;

        var x1 = basis1.Final;
        var x2 = basis2.Final;

        var massFactor1 = 2.0 * bubble2.mass / (bubble1.mass + bubble2.mass);
        var massFactor2 = 2.0 * bubble1.mass / (bubble1.mass + bubble2.mass);

        var vFactor1 = v1.Sub(v2).Dot(x1.Sub(x2)) / Math.Pow(x1.Sub(x2).Magnitude, 2.0);
        var vFactor2 = v2.Sub(v1).Dot(x2.Sub(x1)) / Math.Pow(x2.Sub(x1).Magnitude, 2.0);

        var vPrime1 = v1.Sub(x1.Sub(x2).Mult(massFactor1).Mult(vFactor1));
        var vPrime2 = v2.Sub(x2.Sub(x1).Mult(massFactor2).Mult(vFactor2));

        physics1.velocity = vPrime1.Mult(energyRetention);
        physics2.velocity = vPrime2.Mult(energyRetention);
    }

    private static void PushApart(Basis basis1, Bubble bubble1, Basis basis2, Bubble bubble2)
    {
        var overlap = (bubble1.radius + bubble2.radius) - basis1.Final.DistanceTo(basis2.Final);
        if (overlap < 0)
        {
            return;
        }

        var delta = basis1.Final.Sub(basis2.Final);
        var theta = Math.Atan2(delta.Y, delta.X);

        var offset = new Vector(overlap * Math.Cos(theta), overlap * Math.Sin(theta));
        basis1.home = basis1.home.Plus(offset);
    }
}
