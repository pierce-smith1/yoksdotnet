using SkiaSharp;

using System.Collections.Generic;
using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing.painters;

public static class DebugPainter
{
    public static void DrawDebugInfo(SKCanvas canvas, Sprite sprite)
    {
        var spriteRect = SpritePainter.GetRect(sprite);

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

    private static List<string> InfoToReport(Sprite sprite)
    {
        return sprite is Yokin yokin ?
        [
            $"X: {yokin.home.X}",
            $"Y: {yokin.home.Y}",
        ] : [];
    }
}
