using System;
using System.Collections.Generic;
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
    public PaletteChoice paletteChoice = PaletteChoice.SingleGroup(PaletteGroup.XpInspired);

    public double spriteScale = 0.5;
    public double emotionScale = 0.5;
    public bool trailsEnabled = false;
    public double trailLength = 0.1;

    public double animationSpeed = 0.2;
    public List<Pattern> possiblePatterns = [..SfEnums.GetAll<Pattern>()];
    public PatternChoice startingPattern = PatternChoice.Random();
    public bool patternDoesChange = true;
    public double patternChangeFrequency = 0.5;

    // Custom palettes are serialized and de-serialized separately. See OptionsStore.
    [JsonIgnore]
    public List<CustomPaletteSet> customPalettes = [];
}

public record PatternChoice
{
    public bool IsRandom { get; init; } = default;
    public Pattern? SinglePattern { get; init; } = default;

    public static PatternChoice Random() => new()
    {
        IsRandom = true,
    };

    public static PatternChoice Single(Pattern pattern) => new()
    {
        SinglePattern = pattern,
    };

    public T Match<T>(Func<T> whenRandom, Func<Pattern, T> whenSingle)
    {
        if (IsRandom) return whenRandom();
        if (SinglePattern is not null) return whenSingle(SinglePattern);

        throw new NotImplementedException();
    }
}

public record PaletteChoice
{
    public bool IsAllGroups { get; init; } = default;
    public bool IsRandomlyGenerated { get; init; } = default;
    public PaletteGroup? Group { get; init; } = default;
    public CustomPaletteSetEntry? CustomSet { get; init; } = default;

    public static PaletteChoice SingleGroup(PaletteGroup group) => new()
    {
        Group = group,
    };

    public static PaletteChoice All() => new()
    {
        IsAllGroups = true,
    };

    public static PaletteChoice UserDefined(string id, string name) => new()
    {
        CustomSet = new(id, name),
    };

    public static PaletteChoice RandomlyGenerated() => new()
    {
        IsRandomlyGenerated = true,
    };

    public T Match<T>(
        Func<PaletteGroup, T> whenSingleGroup,
        Func<T> whenAll,
        Func<CustomPaletteSetEntry, T> whenUserDefined,
        Func<T> whenGenerated
    ) {
        if (Group is not null) return whenSingleGroup(Group);
        if (IsAllGroups) return whenAll();
        if (CustomSet is not null) return whenUserDefined(CustomSet);
        if (IsRandomlyGenerated) return whenGenerated();

        throw new NotImplementedException();
    }
}

public record CustomPaletteSetEntry(string Id, string Name);
public record CustomPaletteSet(string Id, string Name, List<CustomPaletteEntry> Entries);
public record CustomPaletteEntry(string Name, Palette Palette);
