using System;
using System.Collections.Generic;
using System.Linq;

namespace yoksdotnet.common;

public static class RandomSamplingExtensions
{
    public static T Sample<T>(this Random rng, IEnumerable<T> source)
    {
        if (!source.Any())
        {
            throw new InvalidOperationException("Sample from empty list");
        }

        var index = rng.Next() % source.Count();
        return source.ElementAt(index);
    }

    public static T? SampleOrDefault<T>(this Random rng, IEnumerable<T> source)
    {
        if (!source.Any())
        {
            return default;
        }

        return rng.Sample(source);
    }

    public static T SampleWeighted<T>(this Random rng, IEnumerable<T> source, Func<T, double> weightProvider)
    {
        if (!source.Any())
        {
            throw new InvalidOperationException("Sample from empty list");
        }

        var totalWeight = source.Select(x => weightProvider(x)).Aggregate((a, b) => a + b);
        var target = rng.NextDouble() * totalWeight;

        var runningWeight = 0.0;
        foreach (var item in source)
        {
            runningWeight += weightProvider(item);
            if (runningWeight >= target)
            {
                return item;
            }
        }

        return source.Last();
    }

    public static T SampleExponential<T>(this Random rng, IEnumerable<T> source, double factor) 
    {
        if (!source.Any())
        {
            throw new InvalidOperationException("Sample from empty list");
        }

        factor = Math.Clamp(factor, 0.1, 1.0);

        while (true)
        {
            foreach (var item in source)
            {
                if (rng.NextDouble() < factor)
                {
                    return item;
                }
            }
        }
    }
}
