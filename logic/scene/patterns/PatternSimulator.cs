using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene.patterns;

public abstract class PatternSimulator
{
    public virtual void Init(AnimationContext _ctx) {}
    public virtual void Run(AnimationContext ctx)
    {
        BeforeMove(ctx);

        foreach (var entity in ctx.scene.entities)
        {
            var oldHome = entity.basis.home;

            MoveEntity(ctx, entity);

            if (entity.emotion is { } emotion)
            {
                SpriteMovement.ApplyEmotionShake(ctx, (entity.basis, emotion));
                EmotionSimulator.UpdateEmotions(ctx, (entity.basis, emotion));
            }

            var newHome = entity.basis.home;
            var measuredVelocity = new Vector(
                (newHome.X - oldHome.X) / ctx.scene.lastDtMs, 
                (newHome.Y - oldHome.Y) / ctx.scene.lastDtMs
            );

            var physicsMeasurements = entity.physicsMeasurements ??= new()
            {
                lastVelocity = new(0.0, 0.0),
            };

            physicsMeasurements.lastVelocity = measuredVelocity;

            TrailSimulator.UpdateTrails(ctx, entity);

            if (entity.skin?.style.IsRefined is true)
            {
                GazeSimulator.UpdateGaze(ctx, entity);
            }
        }
    }
    public virtual void End(AnimationContext _ctx) {}
    public virtual void BeforeMove(AnimationContext _ctx) {}

    public abstract void MoveEntity(AnimationContext ctx, Entity entity);
}

