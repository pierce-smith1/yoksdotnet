using System;
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

public class Palette
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

    public RgbColor this[PaletteIndex index]
    {
        get => index == PaletteIndex.Scales ? scales
            : index == PaletteIndex.ScalesHighlight ? scalesHighlight
            : index == PaletteIndex.ScalesShadow ? scalesShadow
            : index == PaletteIndex.Horns ? horns
            : index == PaletteIndex.HornsShadow ? hornsShadow
            : index == PaletteIndex.Eyes ? eyes
            : index == PaletteIndex.Whites ? whites
            : throw new InvalidOperationException();
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
}

public class PredefinedPalette(string name, PaletteGroup group) : Palette, ISfEnum
{
    public static readonly PredefinedPalette Ascent = PaletteConversion.FromHexStrings(
        "Ascent",
        PaletteGroup.XpInspired,
        "#1963c4",
        "#1963c4",
        "#0b407d",
        "#8c96dd",
        "#8c96dd",
        "#ffffff",
        "#506bbc"
    );

    public static readonly PredefinedPalette Autumn = PaletteConversion.FromHexStrings(
        "Autumn",
        PaletteGroup.XpInspired,
        "#db8313",
        "#be9275",
        "#904a08",
        "#574d3c",
        "#904a08",
        "#f3e6e9",
        "#000000"
    );

    public static readonly PredefinedPalette Azul = PaletteConversion.FromHexStrings(
        "Azul",
        PaletteGroup.XpInspired,
        "#1aadd9",
        "#a1d0e5",
        "#296d9e",
        "#4d8db9",
        "#296d9e",
        "#ffffff",
        "#103050"
    );

    public static readonly PredefinedPalette Bliss = PaletteConversion.FromHexStrings(
        "Bliss",
        PaletteGroup.XpInspired,
        "#73981e",
        "#73981e",
        "#3d5317",
        "#6a96f2",
        "#282438",
        "#eaf2ff",
        "#3b73ee"
    );

    public static readonly PredefinedPalette Crystal = PaletteConversion.FromHexStrings(
        "Crystal",
        PaletteGroup.XpInspired,
        "#38399e",
        "#51b7cf",
        "#1c63d8",
        "#434ad9",
        "#0a1a4a",
        "#eaf2ff",
        "#0a1a4a"
    );


    public static readonly PredefinedPalette Aemil = PaletteConversion.FromHexStrings(
        "Aemil",
        PaletteGroup.Fractalthorns,
        "#56eb8e",
        "#84f5c3",
        "#1d9550",
        "#e29a56",
        "#e16a72",
        "#dff9eb",
        "#966336"
    );

    public static readonly PredefinedPalette Loxxe = PaletteConversion.FromHexStrings(
        "Loxxe",
        PaletteGroup.Fractalthorns,
        "#243966",
        "#31487a",
        "#16274d",
        "#122240",
        "#546e78",
        "#a4b2b6",
        "#0b162c"
    );

    public string Name => name;
    public PaletteGroup Group => group;

    public override string ToString() => name;
}

public record PaletteGroup(string Name) : ISfEnum
{
    public static readonly PaletteGroup XpInspired = new("Classic");
    public static readonly PaletteGroup Fractalthorns = new("Fractalthorns Characters");

    public override string ToString() => Name;
}

[JsonDerivedType(typeof(SingleGroup), nameof(SingleGroup))]
[JsonDerivedType(typeof(AllGroups), nameof(AllGroups))]
[JsonDerivedType(typeof(UserDefined), nameof(UserDefined))]
[JsonDerivedType(typeof(ImFeelingLucky), nameof(ImFeelingLucky))]
public record PaletteChoice
{
    public record SingleGroup(PaletteGroup Group) : PaletteChoice;
    public record AllGroups : PaletteChoice;
    public record UserDefined(string SetId, string Name) : PaletteChoice;
    public record ImFeelingLucky : PaletteChoice;
}
