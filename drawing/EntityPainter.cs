using SkiaSharp;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class EntityPainter
{
    public void Draw(SKCanvas canvas, Sprite sprite)
    {
        var bitmap = GetBitmap(sprite);
        var paint = GetPaint(sprite);

        canvas.DrawBitmap(bitmap, sprite.GetRect(), paint);
    }

    private static SKBitmap GetBitmap(Sprite sprite) => sprite.GetBitmap().Resource;
    private static SKPaint GetPaint(Sprite sprite) => sprite.GetPaint();
}
