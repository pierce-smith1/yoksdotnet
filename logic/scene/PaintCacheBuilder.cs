using SkiaSharp;
using System.Collections.Generic;
using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class PaintCacheBuilder(AppendCache<SKPaint> cache)
{
    private readonly Dictionary<Palette, ICacheHandle<SKPaint>> _paletteHandles = [];
    private readonly Dictionary<RgbColor, ICacheHandle<SKPaint>> _solidColorHandles = [];

    public AppendCache<SKPaint> Cache => cache;

    public ICacheHandle<SKPaint> GetBodyPaintHandle(Palette palette)
    {
        if (_paletteHandles.TryGetValue(palette, out var handle))
        {
            return handle;
        }

        var paint = PaletteConversion.ToSkPaint(palette);
        var newHandle = cache.Register(paint);
        _paletteHandles[palette] = newHandle;

        return newHandle;
    }

    public ICacheHandle<SKPaint> GetSolidColorPaintHandle(RgbColor color)
    {
        if (_solidColorHandles.TryGetValue(color, out var handle))
        {
            return handle;
        }

        var paint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = ColorConversion.ToSkColor(color),
        };

        var newHandle = cache.Register(paint);
        _solidColorHandles[color] = newHandle;

        return newHandle;
    }
}
