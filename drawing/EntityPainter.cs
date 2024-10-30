using SkiaSharp;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class EntityPainter
{
    public void Draw(SKCanvas canvas, Sprite entity)
    {
        var (TopLeft, BotRight) = entity.GetBounds();

        canvas.DrawBitmap
        (
            Images.lk,
            new SKRect((float)TopLeft.X, (float)TopLeft.Y, (float)BotRight.X, (float)BotRight.Y),
            entity.Paint
        );
    }
}
