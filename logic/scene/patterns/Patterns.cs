using System;
using System.Text.Json.Serialization;

using yoksdotnet.common;

namespace yoksdotnet.logic.scene.patterns;

public record Pattern(string Name) : ISfEnum
{
    public readonly static Pattern Lattice = new("Lattice");
    public readonly static Pattern Roamers = new("Roamers");
    public readonly static Pattern Waves = new("Waves");

    public override string ToString() => Name;
}

[JsonDerivedType(typeof(RandomPatternChoice), nameof(RandomPatternChoice))]
[JsonDerivedType(typeof(SinglePatternChoice), nameof(SinglePatternChoice))]
public record PatternChoice;
public record RandomPatternChoice : PatternChoice
{
    public override string ToString() => "Random";
}

public record SinglePatternChoice(Pattern Pattern) : PatternChoice
{
    public override string ToString() => Pattern.Name;
    }
