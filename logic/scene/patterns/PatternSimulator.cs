using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.logic.scene.patterns;

public interface IPatternToken;

public interface IPatternSimulator
{
    public IPatternToken InitSimulation(AnimationContext ctx, Entity entity);
    public void Simulate(AnimationContext ctx, Entity entity, IPatternToken token);
    public void EndSimulation(AnimationContext ctx, Entity entity, IPatternToken token);
}

public abstract class PatternSimulator<T> : IPatternSimulator
{
    private record PatternDataToken(T Data) : IPatternToken;

    public IPatternToken InitSimulation(AnimationContext ctx, Entity entity)
    {
        var token = new PatternDataToken(Init(ctx, entity));
        return token;
    }

    public void Simulate(AnimationContext ctx, Entity entity, IPatternToken token)
    {
        var data = (token as PatternDataToken)!.Data;
        Move(ctx, entity, data);
    }

    public void EndSimulation(AnimationContext ctx, Entity entity, IPatternToken token)
    {
        var data = (token as PatternDataToken)!.Data;
        End(ctx, entity, data);

        entity.patternToken = null;
    }

    public abstract T Init(AnimationContext ctx, Entity entity);
    public abstract void Move(AnimationContext ctx, Entity entity, T data);
    public virtual void End(AnimationContext _ctx, Entity _entity, T _data) {}
}

public abstract class SimplePatternSimulator : IPatternSimulator
{
    private record PatternToken : IPatternToken;

    public IPatternToken InitSimulation(AnimationContext _ctx, Entity _entity) => new PatternToken();

    public void Simulate(AnimationContext ctx, Entity entity, IPatternToken _token)
    {
        Move(ctx, entity);
    }

    public void EndSimulation(AnimationContext _ctx, Entity entity, IPatternToken _token) 
    {
        entity.patternToken = null;
    }

    public abstract void Move(AnimationContext ctx, Entity entity);
}
