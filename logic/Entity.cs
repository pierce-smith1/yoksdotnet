using System;
using System.Collections.Generic;

namespace yoksdotnet.logic;

public class Entity
{
    public required PhysicalBasis basis;

    private readonly Dictionary<Type, EntityComponent> _components = [];

    public void Attach<T>(T component) where T : EntityComponent
    {
        _components[typeof(T)] = component;
    }

    public T EnsureHas<T>(Func<T> getComponent) where T : EntityComponent
    {
        if (!_components.TryGetValue(typeof(T), out var value))
        {
            var component = getComponent();
            Attach(component);
            return component;
        }

        return (value as T)!;
    }

    public T? Get<T>() where T : EntityComponent
    {
        return _components.GetValueOrDefault(typeof(T)) as T;
    }

    public bool Remove<T>() where T : EntityComponent
    {
        return _components.Remove(typeof(T));
    }
}
