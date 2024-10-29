using System;
using System.Collections.Generic;

using yoksdotnet.drawing;
using yoksdotnet.logic.scene;

namespace yoksdotnet.logic;

public class ScrOptions
{
    public int EntityCount { get; init; } = 100;
    public double EntityScale { get; init; } = 0.5;

    public PaletteChoice Palette { get; init; } = new PaletteChoice.SingleGroup(PaletteGroup.XpInspired);
    public int ColorsCount { get; init; } = 4;

    public List<PatternId> AvailablePatterns { get; init; } = [.. Enum.GetValues<PatternId>()];
    public PatternId? StartingPattern { get; init; } = PatternId.Roamers;
    public double? PatternChangeSeconds { get; init; } = 10.0;
}
