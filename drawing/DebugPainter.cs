using SkiaSharp;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class DebugPainter
{
    public void DrawDebugInfo(SKCanvas canvas, Sprite sprite)
    {
        var (TopLeft, BotRight) = sprite.GetBounds();

        var paint = new SKPaint();
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = SKColors.White;

        canvas.DrawRect(
            new SKRect((float)TopLeft.X, (float)TopLeft.Y, (float)BotRight.X, (float)BotRight.Y),
            paint
        );
    }
}
