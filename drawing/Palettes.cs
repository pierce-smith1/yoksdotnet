﻿using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;

namespace yoksdotnet.drawing;

public enum PaletteId
{
    Autumn,
    Ascent,
}

public enum PaletteGroup
{
    XpInspired,
}

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

public class PaletteDefinition
{
    public Dictionary<PaletteIndex, Color> Colors { get; init; } = [];
    public PaletteGroup Group { get; init; }

    public PaletteDefinition
    (
        PaletteGroup group,
        string scales,
        string scalesHighlight,
        string scalesShadow,
        string horns,
        string eye,
        string whites,
        string hornsShadow
    )
    {
        Group = group;

        Colors[PaletteIndex.ScalesShadow] = HexStringToColor(scalesShadow);
        Colors[PaletteIndex.HornsShadow] = HexStringToColor(hornsShadow);
        Colors[PaletteIndex.Eye] = HexStringToColor(eye);
        Colors[PaletteIndex.Horns] = HexStringToColor(horns);
        Colors[PaletteIndex.Scales] = HexStringToColor(scales);
        Colors[PaletteIndex.ScalesHighlight] = HexStringToColor(scalesHighlight);
        Colors[PaletteIndex.Whites] = HexStringToColor(whites);
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
}

public static class Palettes
{
    public static readonly Dictionary<PaletteIndex, byte> IndexLuminances = new()
    {
        { PaletteIndex.ScalesShadow, 0 },
        { PaletteIndex.HornsShadow, 40 },
        { PaletteIndex.Eye, 80 },
        { PaletteIndex.Horns, 120 },
        { PaletteIndex.Scales, 160 },
        { PaletteIndex.ScalesHighlight, 200 },
        { PaletteIndex.Whites, 240 },
    };

    public static readonly Dictionary<PaletteId, PaletteDefinition> Definitions = new()
    {
        { PaletteId.Autumn, new(
            PaletteGroup.XpInspired,
            "#db8313",
            "#be9275",
            "#904a08",
            "#574d3c",
            "#904a08",
            "#f3e6e9",
            "#000000"
        )},
        { PaletteId.Ascent, new(
            PaletteGroup.XpInspired,
            "#1963c4",
            "#1963c4",
            "#0b407d",
            "#8c96dd",
            "#8c96dd",
            "#ffffff",
            "#506bbc"
        )}
    };

    private static SKPaint DefinitionToPaint(PaletteDefinition palette)
    {
        var identityTable = Enumerable.Range(0, 256).Select(i => (byte)i);

        var alphaTable = identityTable.ToArray();

        var redTable = identityTable.ToArray();
        var greenTable = identityTable.ToArray();
        var blueTable = identityTable.ToArray();

        foreach (var (index, color) in IndexLuminances)
        {
            redTable[IndexLuminances[index]] = palette.Colors[index].R;
            greenTable[IndexLuminances[index]] = palette.Colors[index].G;
            blueTable[IndexLuminances[index]] = palette.Colors[index].B;
        }

        var filter = SKColorFilter.CreateTable(alphaTable, redTable, greenTable, blueTable);
        var paint = new SKPaint()
        {
            ColorFilter = filter,
        };
        return paint;
    }

    public static readonly Dictionary<PaletteId, SKPaint> Paints = Definitions
        .Select(pair => new KeyValuePair<PaletteId, SKPaint>(pair.Key, DefinitionToPaint(pair.Value)))
        .ToDictionary();
}
