using System;
using System.Collections.Generic;

using yoksdotnet.drawing;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic;

public class ScrOptions
{
    public double SpriteDensity { get; init; } = 0.5;
    public double SpriteScale { get; init; } = 0.5;

    public PaletteChoice PaletteChoice { get; init; } = new PaletteChoice.SingleGroup(PaletteGroup.XpInspired);
    public double ColorsDensity { get; init; } = 0.5;

    public List<PatternId> AvailablePatterns { get; init; } = [.. Enum.GetValues<PatternId>()];
    public PatternId? StartingPattern { get; init; } = null;
    public double? PatternChangeSeconds { get; init; } = 10.0;
}
