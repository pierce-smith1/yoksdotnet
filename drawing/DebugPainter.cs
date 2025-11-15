using SkiaSharp;

using System.Collections.Generic;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class DebugPainter
{
    private List<string> InfoToReport(Sprite sprite)
    {
        return sprite is Yokin yokin ?
        [
            $"X: {yokin.Home.X}",
            $"Y: {yokin.Home.Y}",
        ] : [];
    }

    public void DrawDebugInfo(SKCanvas canvas, Sprite sprite)
    {
        var spriteRect = sprite.GetRect();

        var borderRectPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White
        };

        canvas.DrawRect(spriteRect, borderRectPaint);

        var infoLines = InfoToReport(sprite);
        for (var i = 0; i < infoLines.Count; i++)
        {
            var line = infoLines[i];

            var textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White,
            };

            var yOffset = (i + 1) * 15;
            var textPos = new SKPoint
            {
                X = spriteRect.Location.X,
                Y = spriteRect.Location.Y + spriteRect.Height + yOffset,
            };
            canvas.DrawText(line, textPos, textPaint);
        }
    }
}
