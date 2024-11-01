using SkiaSharp;

using System.Collections.Generic;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class DebugPainter
{
    private List<string> InfoToReport(Sprite sprite)
    {
        return 
        [
            sprite is Yokin yokin1 ? $"A: {yokin1.Emotion.Ambition:0.##}" : "",
            sprite is Yokin yokin2 ? $"E: {yokin2.Emotion.Empathy:0.##}" : "",
            sprite is Yokin yokin3 ? $"O: {yokin3.Emotion.Optimism:0.##}" : "",
        ];
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

            var yOffset = (i + 1) * 20;
            var textPos = new SKPoint
            {
                X = spriteRect.Location.X,
                Y = spriteRect.Location.Y + spriteRect.Height + yOffset,
            };
            canvas.DrawText(line, textPos, textPaint);
        }
    }
}
