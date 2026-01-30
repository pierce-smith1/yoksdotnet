using SkiaSharp;
using System.Linq;
using yoksdotnet.common;

namespace yoksdotnet.drawing;

public static class PaletteConverter
{
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

    public static Palette FromHexStrings(string s, string sh, string ss, string h, string hs, string e, string w)
    {
        var palette = new Palette
        {
            scales = ColorConverter.FromHex(s),
            scalesHighlight = ColorConverter.FromHex(sh),
            scalesShadow = ColorConverter.FromHex(ss),
            horns = ColorConverter.FromHex(h),
            hornsShadow = ColorConverter.FromHex(hs),
            eyes = ColorConverter.FromHex(e),
            whites = ColorConverter.FromHex(w),
        };

        return palette;
    }

    public static PredefinedPalette FromHexStrings(string name, PaletteGroup group, string s, string sh, string ss, string h, string hs, string e, string w)
    {
        var palette = new PredefinedPalette(name, group)
        {
            scales = ColorConverter.FromHex(s),
            scalesHighlight = ColorConverter.FromHex(sh),
            scalesShadow = ColorConverter.FromHex(ss),
            horns = ColorConverter.FromHex(h),
            hornsShadow = ColorConverter.FromHex(hs),
            eyes = ColorConverter.FromHex(e),
            whites = ColorConverter.FromHex(w),
        };

        return palette;
    }
}
