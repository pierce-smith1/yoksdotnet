using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using yoksdotnet.common;

namespace yoksdotnet.drawing;

public record PaletteIndex(string Name, string DisplayName, int Luminance) : ISfEnum
{
    public readonly static PaletteIndex Scales = new(nameof(Scales), "Scales base", 160);
    public readonly static PaletteIndex ScalesHighlight = new(nameof(ScalesHighlight), "Scale highlights", 200);
    public readonly static PaletteIndex ScalesShadow = new(nameof(ScalesShadow), "Scale shadows", 0);
    public readonly static PaletteIndex Horns = new(nameof(Horns), "Horns base", 120);
    public readonly static PaletteIndex Eyes = new(nameof(Eyes), "Pupil color", 80);
    public readonly static PaletteIndex Whites = new(nameof(Whites), "Teeth and eyes", 240);
    public readonly static PaletteIndex HornsShadow = new(nameof(HornsShadow), "Horns shadow", 40);

    public override string ToString() => DisplayName;
};

public record struct RgbColor(byte R, byte G, byte B);
public record struct HslColor(double H, double S, double L);

public class Palette : IEquatable<Palette>
{
    public required RgbColor scales;
    public required RgbColor scalesHighlight;
    public required RgbColor scalesShadow;
    public required RgbColor horns;
    public required RgbColor eyes;
    public required RgbColor whites;
    public required RgbColor hornsShadow;

    public static readonly Palette DefaultPalette = PaletteConversion.FromHexStrings(
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#cccccc",
        "#aaaaaa"
    );

    public static readonly Palette ExamplePalette = PaletteConversion.FromHexStrings(
        "#b0b0b0",
        "#f2f2f2",
        "#525252",
        "#606060",
        "#494949",
        "#ffffff",
        "#383838"
    );

    public RgbColor this[PaletteIndex index]
    {
        get
        {
            if (index == PaletteIndex.Scales) return scales;
            if (index == PaletteIndex.ScalesHighlight) return scalesHighlight;
            if (index == PaletteIndex.ScalesShadow) return scalesShadow;
            if (index == PaletteIndex.Horns) return horns;
            if (index == PaletteIndex.HornsShadow) return hornsShadow;
            if (index == PaletteIndex.Eyes) return eyes;
            if (index == PaletteIndex.Whites) return whites;

            throw new NotSupportedException();
        }
        set
        {
            if (index == PaletteIndex.Scales) scales = value;
            if (index == PaletteIndex.ScalesHighlight) scalesHighlight = value;
            if (index == PaletteIndex.ScalesShadow) scalesShadow = value;
            if (index == PaletteIndex.Horns) horns = value;
            if (index == PaletteIndex.HornsShadow) hornsShadow = value;
            if (index == PaletteIndex.Eyes) eyes = value;
            if (index == PaletteIndex.Whites) whites = value;
        }
    }

    public bool Equals(Palette? other)
    {
        if (other is null)
        {
            return false;
        }

        var equal = scales == other.scales
            && scalesHighlight == other.scalesHighlight
            && scalesShadow == other.scalesHighlight
            && horns == other.horns
            && hornsShadow == other.hornsShadow
            && eyes == other.eyes
            && whites == other.whites;

        return equal;
    }

    public override int GetHashCode()
    {
        var code = scales.GetHashCode()
            ^ scalesHighlight.GetHashCode()
            ^ scalesShadow.GetHashCode()
            ^ horns.GetHashCode()
            ^ hornsShadow.GetHashCode()
            ^ eyes.GetHashCode()
            ^ whites.GetHashCode();

        return code;
    }

    public override bool Equals(object? other) => Equals(other as Palette);
}

public record PaletteGroup(string Name) : ISfEnum
{
    public static readonly PaletteGroup XpInspired = new("Neptune");
    public static readonly PaletteGroup SevenInspired = new("Blackcomb");
    public static readonly PaletteGroup Fractalthorns = new("Fractalthorns");

    public override string ToString() => Name;
}

public class JsonRgbColorConverter : JsonConverter<RgbColor>
{
    public override RgbColor Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions _options)
    {
        var input = reader.GetString()!;

        var color = ColorConversion.FromHex(input);
        if (color is null)
        {
            throw new JsonException();
        }

        return color.Value;
    }

    public override void Write(Utf8JsonWriter writer, RgbColor color, JsonSerializerOptions _options)
    {
        var serialized = ColorConversion.ToHex(color);
        writer.WriteStringValue(serialized);
    }
}
