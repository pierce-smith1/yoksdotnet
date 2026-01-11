using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using yoksdotnet.common;

namespace yoksdotnet.drawing;

public record PaletteIndex(string Name, string DisplayName, int Luminance) : IStaticFieldEnumeration
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

public record Color(byte R, byte G, byte B)
{
    public static Color FromHex(string hex)
    {
        if (hex.StartsWith('#'))
        {
            hex = hex[1..];
        }

        var red = Convert.ToByte(hex[..2], 16);
        var green = Convert.ToByte(hex[2..4], 16);
        var blue = Convert.ToByte(hex[4..6], 16);

        return new(red, green, blue);
    }

    public (double H, double S, double L) ToHsl()
    {
        var rp = R / 255.0;
        var gp = G / 255.0;
        var bp = B / 255.0;

        var cmax = new[] { rp, gp, bp }.Max();
        var cmin = new[] { rp, gp, bp }.Min();
        var delta = cmax - cmin;

        var h = (delta == 0) ? 0
            : (cmax == rp) ? 60 * (((gp - bp) / delta) % 6)
            : (cmax == gp) ? 60 * (((bp - rp) / delta) + 2)
            : (cmax == bp) ? 60 * (((rp - gp) / delta) + 4)
            : 0
            ;

        while (h < 0)
        {
            h += 360;
        }

        var l = (cmax + cmin) / 2;

        var s = (delta == 0) ? 0
            : delta / (1 - Math.Abs(2 * l - 1))
            ;

        return (h, s * 100, l * 100);
    }

    public string AsHex()
    {
        var hex = $"#{R:x2}{G:x2}{B:x2}";
        return hex;
    }

    public static Color FromHsl(double h, double s, double l)
    {
        s /= 100;
        l /= 100;

        var c = (1 - Math.Abs(2 * l - 1)) * s;
        var x = c * (1 - Math.Abs((h / 60 % 2) - 1));
        var m = l - c / 2;

        var (rp, gp, bp) = h switch
        {
            < 60 => (c, x, 0),
            >= 60 and < 120 => (x, c, 0),
            >= 120 and < 180 => (0, c, x),
            >= 180 and < 240 => (0, x, c),
            >= 240 and < 300 => (x, 0, c),
            >= 300 => (c, 0, x),
            _ => (0.0, 0.0, 0.0)
        };

        var r = rp + m;
        var g = gp + m;
        var b = bp + m;

        return new((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }
};

public class Palette
{
    public Dictionary<PaletteIndex, Color> Colors = [];

    public Color Scales
    {
        get => Colors[PaletteIndex.Scales];
        set => Colors[PaletteIndex.Scales] = value;
    }
    public Color ScalesHighlight
    {
        get => Colors[PaletteIndex.ScalesHighlight];
        set => Colors[PaletteIndex.ScalesHighlight] = value;
    }
    public Color ScalesShadow
    {
        get => Colors[PaletteIndex.ScalesShadow];
        set => Colors[PaletteIndex.ScalesShadow] = value;
    }
    public Color Horns
    {
        get => Colors[PaletteIndex.Horns];
        set => Colors[PaletteIndex.Horns] = value;
    }
    public Color Eyes
    {
        get => Colors[PaletteIndex.Eyes];
        set => Colors[PaletteIndex.Eyes] = value;
    }
    public Color Whites
    {
        get => Colors[PaletteIndex.Whites];
        set => Colors[PaletteIndex.Whites] = value;
    }
    public Color HornsShadow
    {
        get => Colors[PaletteIndex.HornsShadow];
        set => Colors[PaletteIndex.HornsShadow] = value;
    }


    public Palette()
        : this("#000000", "#ff00ff", "#ff00ff", "#000000", "#ff00ff", "#000000", "#ff00ff") 
    { }

    public Palette(Color s, Color sh, Color ss, Color h, Color e, Color w, Color hs)
    {
        Colors[PaletteIndex.Scales] = s;
        Colors[PaletteIndex.ScalesHighlight] = sh;
        Colors[PaletteIndex.ScalesShadow] = ss;
        Colors[PaletteIndex.Horns] = h;
        Colors[PaletteIndex.Eyes] = e;
        Colors[PaletteIndex.Whites] = w;
        Colors[PaletteIndex.HornsShadow] = hs;
    }
    public Palette(string s, string sh, string ss, string h, string e, string w, string hs)
    {
        Colors[PaletteIndex.Scales] = Color.FromHex(s);
        Colors[PaletteIndex.ScalesHighlight] = Color.FromHex(sh);
        Colors[PaletteIndex.ScalesShadow] = Color.FromHex(ss);
        Colors[PaletteIndex.Horns] = Color.FromHex(h);
        Colors[PaletteIndex.Eyes] = Color.FromHex(e);
        Colors[PaletteIndex.Whites] = Color.FromHex(w);
        Colors[PaletteIndex.HornsShadow] = Color.FromHex(hs);
    }

    public Palette(Palette other)
    {
        Colors = new(other.Colors);
    }

    public Color this[PaletteIndex index]
    {
        get => Colors[index];
        set => Colors[index] = value;
    }

    public SKPaint GetPaint()
    {
        var identityTable = Enumerable.Range(0, 256).Select(i => (byte)i);

        var alphaTable = identityTable.ToArray();

        var redTable = identityTable.ToArray();
        var greenTable = identityTable.ToArray();
        var blueTable = identityTable.ToArray();

        foreach (var index in StaticFieldEnumerations.GetAll<PaletteIndex>())
        {
            redTable[index.Luminance] = Colors[index].R;
            greenTable[index.Luminance] = Colors[index].G;
            blueTable[index.Luminance] = Colors[index].B;
        }

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

    private PredefinedPalette(string displayName, PaletteGroup group, string s, string sh, string ss, string h, string e, string w, string hs)
        : base(s, sh, ss, h, e, w, hs)
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
