using SkiaSharp;
using System;
using System.Linq;
using yoksdotnet.common;

namespace yoksdotnet.logic.scene.patterns;

public class Bubble : EntityComponent
{
    public required double radius;
    public bool isFree = false;
}

public static class BubblesPattern
{
    public static void Start(AnimationContext ctx, Entity entity, Brand brand)
    {
        var physics = entity.EnsureHas<Physics>(() => new()
        {
            velocity = Vector.RandomScaled(ctx.rng, 2.0, 2.0),
            mass = Interp.Linear(ctx.rng.NextDouble(), 0.0, 1.0, 0.5, 1.5),
        });

        var bubble = entity.EnsureHas<Bubble>(() => new()
        {
            radius = 20.0 + physics.mass * 10.0,
        });

        bubble.isFree = false;
    }

    public static void Move(AnimationContext ctx, Entity entity, Brand brand)
    {
        if (entity.Get<Bubble>() is not { } bubble)
        {
            return;
        }

        if (entity.Get<Physics>() is not { } physics)
        {
            return;
        }

        var collidingPeers = ctx.scene.entities.Where(e => AreColliding(entity, e));
        foreach (var peer in collidingPeers)
        {
            var peerBubble = peer.Get<Bubble>()!;

            if (bubble.isFree && peerBubble.isFree)
            {
                SimulateCollision(entity.basis, physics, peer.basis, peer.Get<Physics>()!);
                PushApart(entity.basis, bubble, peer.basis, peerBubble);
            } 
        }

        if (!collidingPeers.Any())
        {
            bubble.isFree = true;
        }

        ApplyDrift(ctx, physics, brand);

        SpriteMovement.SimulatePhysics(ctx, entity.basis, physics);
        BouncyPattern.BounceOffScreen(ctx, entity.basis, physics);
    }

    private static void ApplyDrift(AnimationContext ctx, Physics physics, Brand brand)
    {
        var driftX = Math.Sin(ctx.scene.seconds * 2.0 + brand.value * 10.0) / 200.0;
        var driftY = Math.Cos(ctx.scene.seconds * 3.0 + brand.value * 10.0) / 200.0;

        physics.velocity.X += driftX;
        physics.velocity.Y += driftY;
    }

    private static bool AreColliding(Entity e1, Entity e2)
    {
        if (e1 == e2)
        {
            return false;
        }

        if (e1.Get<Bubble>() is not { } b1)
        {
            return false;
        }

        if (e2.Get<Bubble>() is not { } b2)
        {
            return false;
        }

        var isColliding = e1.basis.Final.DistanceTo(e2.basis.Final) <= b1.radius + b2.radius;
        return isColliding;
    }

    private static void SimulateCollision(PhysicalBasis b1, Physics p1, PhysicalBasis b2, Physics p2)
    {
        const double energyRetention = 0.95;

        var v1 = p1.velocity;
        var v2 = p2.velocity;

        var x1 = b1.Final;
        var x2 = b2.Final;

        var massFactor1 = (2 * p2.mass) / (p1.mass + p2.mass);
        var massFactor2 = (2 * p1.mass) / (p1.mass + p2.mass);

        var vFactor1 = v1.Sub(v2).Dot(x1.Sub(x2)) / Math.Pow(x1.Sub(x2).Magnitude, 2);
        var vFactor2 = v2.Sub(v1).Dot(x2.Sub(x1)) / Math.Pow(x2.Sub(x1).Magnitude, 2);

        var vPrime1 = v1.Sub(x1.Sub(x2).Mult(massFactor1).Mult(vFactor1));
        var vPrime2 = v2.Sub(x2.Sub(x1).Mult(massFactor2).Mult(vFactor2));

        p1.velocity = vPrime1.Mult(energyRetention);
        p2.velocity = vPrime2.Mult(energyRetention);
    }

    private static void PushApart(PhysicalBasis basis1, Bubble bubble1, PhysicalBasis basis2, Bubble bubble2)
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
