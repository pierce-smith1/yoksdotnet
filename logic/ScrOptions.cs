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
    public PaletteChoice paletteChoice = new SingleGroupPalettes(PaletteGroup.XpInspired);

    public double spriteScale = 0.5;
    public double emotionScale = 0.5;
    public bool trailsEnabled = false;
    public double trailLength = 0.1;

    public double animationSpeed = 0.2;
    public List<Pattern> possiblePatterns = [..SfEnums.GetAll<Pattern>()];
    public PatternChoice startingPattern = new RandomPatternChoice();
    public bool patternDoesChange = true;
    public double patternChangeFrequency = 0.5;

    // Custom palettes are serialized and de-serialized separately. See OptionsStore.
    [JsonIgnore]
    public List<CustomPaletteSet> customPalettes = [];
}

[JsonDerivedType(typeof(RandomPatternChoice), nameof(RandomPatternChoice))]
[JsonDerivedType(typeof(SinglePatternChoice), nameof(SinglePatternChoice))]
public abstract record PatternChoice : Union<RandomPatternChoice, SinglePatternChoice>;
public record RandomPatternChoice : PatternChoice;
public record SinglePatternChoice(Pattern Pattern) : PatternChoice;


[JsonDerivedType(typeof(SingleGroupPalettes), nameof(SingleGroupPalettes))]
[JsonDerivedType(typeof(AllPalettes), nameof(AllPalettes))]
[JsonDerivedType(typeof(UserDefinedPalettes), nameof(UserDefinedPalettes))]
[JsonDerivedType(typeof(GeneratedPalettes), nameof(GeneratedPalettes))]
public abstract record PaletteChoice : Union<SingleGroupPalettes, AllPalettes, UserDefinedPalettes, GeneratedPalettes>;
public record SingleGroupPalettes(PaletteGroup Group) : PaletteChoice;
public record AllPalettes : PaletteChoice;
public record UserDefinedPalettes(string SetId, string Name) : PaletteChoice;
public record GeneratedPalettes : PaletteChoice;

public record CustomPaletteSet(string Id, string Name, List<CustomPaletteEntry> Entries);
public record CustomPaletteEntry(string Name, Palette Palette);
