using SkiaSharp;

using System.Collections.Generic;
using yoksdotnet.logic;

namespace yoksdotnet.drawing.painters;

public static class DebugPainter
{
    public static void DrawDebugInfo(SKCanvas canvas, Entity entity)
    {
        var spriteRect = SpritePainter.GetRect(entity);

        var borderRectPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White
        };

        canvas.DrawRect(spriteRect, borderRectPaint);

        var infoLines = InfoToReport(entity);
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

    private static List<string> InfoToReport(Entity entity)
    {
        return [
            $"A: {entity.emotion?.ambition}",
            $"E: {entity.emotion?.empathy}",
            $"O: {entity.emotion?.optimism}",
        ];
    }
}
