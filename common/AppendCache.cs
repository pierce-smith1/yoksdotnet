using System.Collections.Generic;

namespace yoksdotnet.common;

public interface ICacheHandle<T>
{
    int Index { get; init; }
}


public class AppendCache<T>
{
    private record CacheHandle<U>(int Index) : ICacheHandle<T>;

    private readonly List<T> _objects = [];

    public ICacheHandle<T> Register(T item)
    {
        var index = _objects.Count;
        _objects.Add(item);
        return new CacheHandle<T>(index);
    }

    public T Get(ICacheHandle<T> handle)
    {
        return _objects[handle.Index];
    }
}
