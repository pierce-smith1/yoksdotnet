using System;
using System.Collections.Generic;
using System.Linq;

using yoksdotnet.logic.scene;

namespace yoksdotnet.logic;

public class ScrOptions
{
    public int EntityCount { get; init; } = 100;
    public double EntityScale { get; init; } = 0.5;

    public int ColorsCount { get; init; } = 2;

    public List<PatternId> AvailablePatterns { get; init; } = Enum.GetValues<PatternId>().ToList();
    public double? PatternChangeSeconds { get; init; } = 10.0;
}
