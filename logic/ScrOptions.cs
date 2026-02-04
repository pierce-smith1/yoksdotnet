using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using yoksdotnet.common;
using yoksdotnet.drawing;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic;

public class ScrOptions
{
    public int version = 1;

    public double diversity = 0.2;
    public double familySize = 0.5;
    public double impostorDensity = 0.1;
    public PaletteChoice paletteChoice = new PaletteChoice.SingleGroup(PaletteGroup.XpInspired);

    public double spriteScale = 0.5;
    public double emotionScale = 0.5;
    public bool trailsEnabled = false;
    public double trailLength = 0.1;

    public double animationSpeed = 0.2;
    public List<Pattern> possiblePatterns = [.. SfEnums.GetAll<Pattern>()];
    public PatternChoice startingPattern = new RandomPatternChoice();
    public bool patternDoesChange = true;
    public double patternChangeFrequency = 0.5;

    // Custom palettes are serialized and de-serialized separately. See OptionsStore.
    [JsonIgnore]
    public List<CustomPaletteSet> customPalettes = [];
}

public record CustomPaletteSet(string Id, string Name, List<CustomPaletteEntry> Entries);
public record CustomPaletteEntry(string Name, Palette Palette);
