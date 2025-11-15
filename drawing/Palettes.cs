using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using yoksdotnet.common;

namespace yoksdotnet.drawing;

public record Color(byte R, byte G, byte B);
public record PaletteColors
(
    Color Scales,
    Color ScalesHighlight,
    Color ScalesShadow,
    Color Horns,
    Color Eyes,
    Color Whites,
    Color HornsShadow
) {
    public PaletteColors
    (
        string scales,
        string scalesHighlight,
        string scalesShadow,
        string horns,
        string eye,
        string whites,
        string hornsShadow
    ) : this(
        ScalesShadow: HexStringToColor(scalesShadow),
        HornsShadow: HexStringToColor(hornsShadow),
        Eyes: HexStringToColor(eye),
        Horns: HexStringToColor(horns),
        Scales: HexStringToColor(scales),
        ScalesHighlight: HexStringToColor(scalesHighlight),
        Whites: HexStringToColor(whites)
    ) { }

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
};

public class Palette
{
    public PaletteColors Colors { get; init; }
    public SKPaint Paint { get; init; }
    
    public Palette(PaletteColors colors)
    {
        Colors = colors;
        Paint = GetPaint();
    }

    private SKPaint GetPaint()
    {
        var identityTable = Enumerable.Range(0, 256).Select(i => (byte)i);

        var alphaTable = identityTable.ToArray();

        var redTable = identityTable.ToArray();
        var greenTable = identityTable.ToArray();
        var blueTable = identityTable.ToArray();

        redTable[0] = Colors.ScalesShadow.R;
        greenTable[0] = Colors.ScalesShadow.G;
        blueTable[0] = Colors.ScalesShadow.B;

        redTable[40] = Colors.HornsShadow.R;
        greenTable[40] = Colors.HornsShadow.G;
        blueTable[40] = Colors.HornsShadow.B;

        redTable[80] = Colors.Eyes.R;
        greenTable[80] = Colors.Eyes.G;
        blueTable[80] = Colors.Eyes.B;

        redTable[120] = Colors.Horns.R;
        greenTable[120] = Colors.Horns.G;
        blueTable[120] = Colors.Horns.B;

        redTable[160] = Colors.Scales.R;
        greenTable[160] = Colors.Scales.G;
        blueTable[160] = Colors.Scales.B;

        redTable[200] = Colors.ScalesHighlight.R;
        greenTable[200] = Colors.ScalesHighlight.G;
        blueTable[200] = Colors.ScalesHighlight.B;
        
        redTable[240] = Colors.Whites.R;
        greenTable[240] = Colors.Whites.G;
        blueTable[240] = Colors.Whites.B;

        var filter = SKColorFilter.CreateTable(alphaTable, redTable, greenTable, blueTable);
        var paint = new SKPaint()
        {
            ColorFilter = filter,
        };
        return paint;
    }
}

public class PredefinedPalette : Palette, IStaticFieldEnumeration
{
    public static readonly PredefinedPalette Ascent = new(
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

    public static readonly PredefinedPalette Autumn = new(
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

    public static readonly PredefinedPalette Azul = new(
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

    public static readonly PredefinedPalette Bliss = new(
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

    public static readonly PredefinedPalette Crystal = new(
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


    public static readonly PredefinedPalette Aemil = new(
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

    public static readonly PredefinedPalette Loxxe = new(
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

    public PaletteGroup Group { get; init; }
    public string Name { get; init; }

    private PredefinedPalette
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
    )
        : base(new PaletteColors
        (
            scales: scales,
            scalesHighlight: scalesHighlight,
            scalesShadow:
            scalesShadow,
            horns: horns,
            eye: eye,
            whites: whites,
            hornsShadow: hornsShadow
        ))
    {
        Group = group;
        Name = displayName;
    }

    public static IEnumerable<PredefinedPalette> AllForGroup(PaletteGroup group)
    {
        return StaticFieldEnumerations.GetAll<PredefinedPalette>().Where(p => p.Group == group);
    }

    public override string ToString() => Name;
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
