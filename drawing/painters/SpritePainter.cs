using SkiaSharp;
using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing.painters;

public static class SpritePainter
{
    public static void Draw(SKCanvas canvas, Sprite sprite)
    {
        var skBitmap = SpriteBitmaps.GetBitmap(sprite).Resource;
        var skPaint = sprite.addons.cachedPaint ?? PaletteConversion.ToSkPaint(sprite.palette);

        canvas.DrawBitmap(skBitmap, GetRect(sprite), skPaint);
    }

    public static SKRect GetRect(Sprite sprite)
    {
        var (topLeft, bottomRight) = sprite.Bounds;

        var rect = new SKRect((float)topLeft.X, (float)topLeft.Y, (float)bottomRight.X, (float)bottomRight.Y);
        return rect;
    }
}
