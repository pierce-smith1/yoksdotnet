using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using yoksdotnet.common;

namespace yoksdotnet.drawing;

public enum PaletteIndex
{
    ScalesShadow,
    HornsShadow,
    Eye,
    Horns,
    Scales,
    ScalesHighlight,
    Whites
}

public record Color(byte R, byte G, byte B);

public class Palette : IStaticFieldEnumeration
{
    public static readonly Palette Ascent = new(
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

    public static readonly Palette Autumn = new(
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

    public static readonly Palette Azul = new(
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

    public static readonly Palette Bliss = new(
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

    public static readonly Palette Crystal = new(
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


    public static readonly Palette Aemil = new(
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

    public static readonly Palette Loxxe = new(
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

    public string Name { get; init; }
    public Dictionary<PaletteIndex, Color> Colors { get; init; } = [];
    public PaletteGroup Group { get; init; }
    public SKPaint Paint { get; init; }

    private Palette
    (
        string displayName,
        PaletteGroup group,
        string scales,
        string scalesHighlight,
        string scalesShadow,
        string horns,
        string eye,
        string whites,
        string hornsShadow
    ) {
        Name = displayName;
        Group = group;

        Colors[PaletteIndex.ScalesShadow] = HexStringToColor(scalesShadow);
        Colors[PaletteIndex.HornsShadow] = HexStringToColor(hornsShadow);
        Colors[PaletteIndex.Eye] = HexStringToColor(eye);
        Colors[PaletteIndex.Horns] = HexStringToColor(horns);
        Colors[PaletteIndex.Scales] = HexStringToColor(scales);
        Colors[PaletteIndex.ScalesHighlight] = HexStringToColor(scalesHighlight);
        Colors[PaletteIndex.Whites] = HexStringToColor(whites);

        Paint = GetPaint();
    }

    private static Color HexStringToColor(string hex)
    {
        if (hex.StartsWith('#'))
        {
            hex = hex.Substring(1);
        }

        var red = Convert.ToByte(hex.Substring(0, 2), 16);
        var green = Convert.ToByte(hex.Substring(2, 2), 16);
        var blue = Convert.ToByte(hex.Substring(4, 2), 16);

        return new(red, green, blue);
    }

    private static readonly Dictionary<PaletteIndex, byte> IndexLuminances = new()
    {
        { PaletteIndex.ScalesShadow, 0 },
        { PaletteIndex.HornsShadow, 40 },
        { PaletteIndex.Eye, 80 },
        { PaletteIndex.Horns, 120 },
        { PaletteIndex.Scales, 160 },
        { PaletteIndex.ScalesHighlight, 200 },
        { PaletteIndex.Whites, 240 },
    };

    private SKPaint GetPaint()
    {
        var identityTable = Enumerable.Range(0, 256).Select(i => (byte)i);

        var alphaTable = identityTable.ToArray();

        var redTable = identityTable.ToArray();
        var greenTable = identityTable.ToArray();
        var blueTable = identityTable.ToArray();

        foreach (var (index, color) in IndexLuminances)
        {
            redTable[IndexLuminances[index]] = Colors[index].R;
            greenTable[IndexLuminances[index]] = Colors[index].G;
            blueTable[IndexLuminances[index]] = Colors[index].B;
        }

        var filter = SKColorFilter.CreateTable(alphaTable, redTable, greenTable, blueTable);
        var paint = new SKPaint()
        {
            ColorFilter = filter,
        };
        return paint;
    }

    public override string ToString()
    {
        return Name;
    }
}

public class PaletteGroup : IStaticFieldEnumeration
{
    public static readonly PaletteGroup XpInspired = new("Classic");
    public static readonly PaletteGroup Fractalthorns = new("Fractalthorns Characters");

    public string Name { get; init; }

    private PaletteGroup(string displayName) 
    {
        Name = displayName;
    }

    public override string ToString()
    {
        return Name;
    }
}

[JsonDerivedType(typeof(SingleGroup), nameof(SingleGroup))]
[JsonDerivedType(typeof(AllGroups), nameof(AllGroups))]
[JsonDerivedType(typeof(UserDefined), nameof(UserDefined))]
[JsonDerivedType(typeof(ImFeelingLucky), nameof(ImFeelingLucky))]
public record PaletteChoice
{
    public record SingleGroup(PaletteGroup Group) : PaletteChoice()
    {
        public override string ToString() => Group.ToString();
    }

    public record AllGroups() : PaletteChoice()
    {
        public override string ToString() => "Everything";
    };

    public record UserDefined() : PaletteChoice()
    {
        public override string ToString() => "Custom";
    }

    public record ImFeelingLucky() : PaletteChoice()
    {
        public override string ToString() => "I'm Feeling Lucky";
    }
}
