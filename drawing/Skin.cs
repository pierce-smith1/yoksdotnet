using SkiaSharp;
using yoksdotnet.common;
using yoksdotnet.logic;

namespace yoksdotnet.drawing;

public class Skin : EntityComponent
{
    public required Palette palette;
    public required SpriteStyleChoice style;
    public Bitmap? fixedBitmap = null;

    public AppendCache<SKPaint>? paintCache;
    public ICacheHandle<SKPaint>? whitePaintHandle;
    public ICacheHandle<SKPaint>? eyePaintHandle;
    public ICacheHandle<SKPaint>? bodyPaintHandle;

    public SKPaint BodyPaint => (bodyPaintHandle is not null && paintCache is not null)
        ? paintCache.Get(bodyPaintHandle)
        : PaletteConversion.ToSkPaint(palette);

    public SKPaint WhitesPaint => (whitePaintHandle is not null && paintCache is not null)
        ? paintCache.Get(whitePaintHandle)
        : new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = ColorConversion.ToSkColor(palette.whites),
        };

    public SKPaint EyePaint => (eyePaintHandle is not null && paintCache is not null)
        ? paintCache.Get(eyePaintHandle)
        : new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = ColorConversion.ToSkColor(palette.eyes),
        };
}
