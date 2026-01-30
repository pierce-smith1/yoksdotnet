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

[JsonDerivedType(typeof(Random), nameof(Random))]
[JsonDerivedType(typeof(SinglePattern), nameof(SinglePattern))]
public record PatternChoice
{
    public record Random() : PatternChoice()
    {
        public override string ToString() => "Random";
    }

    public record SinglePattern(Pattern Pattern) : PatternChoice()
    {
        public override string ToString() => Pattern.Name;
    }
}
