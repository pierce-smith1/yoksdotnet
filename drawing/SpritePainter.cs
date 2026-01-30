using SkiaSharp;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class SpritePainter(Sprite sprite, Palette palette)
{
    private SKPaint _paint = PaletteConverter.ToSkPaint(palette);

    public void Draw(SKCanvas canvas, Bitmap bitmap)
    {
        var skBitmap = bitmap.Resource;

        canvas.DrawBitmap(skBitmap, GetRect(sprite), _paint);
    }

    public void RefreshPaint()
    {
        _paint = PaletteConverter.ToSkPaint(palette);
    }

    public static SKRect GetRect(Sprite sprite)
    {
        var (topLeft, bottomRight) = sprite.Bounds;

        var rect = new SKRect((float)topLeft.X, (float)topLeft.Y, (float)bottomRight.X, (float)bottomRight.Y);
        return rect;
    }
}
