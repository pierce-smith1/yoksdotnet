using System;
using System.Collections.Generic;
using System.Linq;

namespace yoksdotnet.common;

public class RandomSampler(Random rng)
{
    public T? Sample<T>(IEnumerable<T> source) where T : class
    {
        if (!source.Any())
        {
            return null;
        }

        var index = rng.Next() % source.Count();
        return source.ElementAt(index);
    }

    public T? SampleWeighted<T>(IEnumerable<T> source, Func<T, double> weightProvider) where T : class
    {
        if (!source.Any())
        {
            return null;
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

    public T? SampleExponential<T>(IEnumerable<T> source, double factor) where T : class
    {
        if (!source.Any())
        {
            return null;
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
