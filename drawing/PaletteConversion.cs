using SkiaSharp;
using System.Linq;
using yoksdotnet.common;

namespace yoksdotnet.drawing;

public static class PaletteConversion
{
    public static Palette Copy(Palette palette)
    {
        var copy = new Palette
        {
            scales = palette.scales,
            scalesHighlight = palette.scalesHighlight,
            scalesShadow = palette.scalesShadow,
            horns = palette.horns,
            hornsShadow = palette.hornsShadow,
            eyes = palette.eyes,
            whites = palette.whites,
        };

        return copy;
    }

    public static SKPaint ToSkPaint(Palette palette)
    {
        var identityTable = Enumerable.Range(0, 256).Select(i => (byte)i);

        var alphaTable = identityTable.ToArray();

        var redTable = identityTable.ToArray();
        var greenTable = identityTable.ToArray();
        var blueTable = identityTable.ToArray();

        foreach (var index in SfEnums.GetAll<PaletteIndex>())
        {
            redTable[index.Luminance] = palette[index].R;
            greenTable[index.Luminance] = palette[index].G;
            blueTable[index.Luminance] = palette[index].B;
        }

        var filter = SKColorFilter.CreateTable(alphaTable, redTable, greenTable, blueTable);
        var paint = new SKPaint()
        {
            ColorFilter = filter,
        };
        return paint;
    }

    public static Palette FromHexStrings(string s, string sh, string ss, string h, string e, string w, string hs)
    {
        var palette = new Palette
        {
            scales = ColorConversion.FromHexCertain(s),
            scalesHighlight = ColorConversion.FromHexCertain(sh),
            scalesShadow = ColorConversion.FromHexCertain(ss),
            horns = ColorConversion.FromHexCertain(h),
            hornsShadow = ColorConversion.FromHexCertain(hs),
            eyes = ColorConversion.FromHexCertain(e),
            whites = ColorConversion.FromHexCertain(w),
        };

        return palette;
    }

    public static PredefinedPalette FromHexStrings(string name, PaletteGroup group, string s, string sh, string ss, string h, string e, string w, string hs)
    {
        var palette = new PredefinedPalette(name, group)
        {
            scales = ColorConversion.FromHexCertain(s),
            scalesHighlight = ColorConversion.FromHexCertain(sh),
            scalesShadow = ColorConversion.FromHexCertain(ss),
            horns = ColorConversion.FromHexCertain(h),
            hornsShadow = ColorConversion.FromHexCertain(hs),
            eyes = ColorConversion.FromHexCertain(e),
            whites = ColorConversion.FromHexCertain(w),
        };

        return palette;
    }
}
