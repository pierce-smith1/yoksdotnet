using System;
using System.Collections.Generic;
using System.Linq;

namespace yoksdotnet.common;

public static class RandomUtils
{
    public static readonly Random SharedRng = new();

    public static T Sample<T>(this Random rng, IEnumerable<T> source)
    {
        if (! source.Any())
        {
            throw new InvalidOperationException("Sample from empty list");
        }

        var index = rng.Next() % source.Count();
        return source.ElementAt(index);
    }

    public static EnumType SampleEnum<EnumType>(this Random rng) where EnumType : Enum
    {
        var enumValues = Enum.GetValues(typeof(EnumType)).Cast<EnumType>();
        return rng.Sample(enumValues);
    }
}
