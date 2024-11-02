using SkiaSharp;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class EntityPainter
{
    public void Draw(SKCanvas canvas, Sprite sprite)
    {
        canvas.DrawBitmap(Bitmaps.Lk, sprite.GetRect(), sprite.Paint);
    }
}
