using SkiaSharp;

using System.Collections.Generic;
using yoksdotnet.data;
using yoksdotnet.data.entities;

namespace yoksdotnet.drawing.painters;

public static class DebugPainter
{
    private static SKPaint _textPaint = new()
    {
        Style = SKPaintStyle.Fill,
        Color = SKColors.White,
    };
    private static SKFont _textFont = new();

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

            var yOffset = (i + 1) * 15;
            var textPos = new SKPoint
            {
                X = spriteRect.Location.X,
                Y = spriteRect.Location.Y + spriteRect.Height + yOffset,
            };
            canvas.DrawText(line, textPos, SKTextAlign.Left, _textFont, _textPaint);
        }

        DebugBoid(canvas, entity);
    }

    public static void DrawDiagnostics(SKCanvas canvas, AnimationContext ctx)
    {
        List<string> debugLines = [
            $"compute: {ctx.computeStopwatch.Elapsed.Milliseconds} ms",
            $"render: {ctx.renderStopwatch.Elapsed.Milliseconds} ms"
        ];

        var debugLineHeight = 14.0f;
        var debugLineMargin = 20.0f;

        canvas.Save();

        canvas.ClipRect(new SKRect(0.0f, 0.0f, 300.0f, debugLineHeight * debugLines.Count + debugLineMargin));
        canvas.Clear(SKColors.Black);

        canvas.Restore();

        for (var i = 0; i < debugLines.Count; i++)
        {
            var line = debugLines[i];
            canvas.DrawText(line, new SKPoint(debugLineMargin, debugLineMargin + i * debugLineHeight), SKTextAlign.Left, _textFont, _textPaint);
        }
    }

    private static void DebugBoid(SKCanvas canvas, Entity entity)
    {
        if (entity.boid is not { } boid)
        {
            return;
        }

        var avoidRadiusPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
        };

        canvas.DrawCircle((float)entity.basis.Final.X, (float)entity.basis.Final.Y, (float)boid.avoidRadius, avoidRadiusPaint);
    }

    private static List<string> InfoToReport(Entity entity)
    {
        return [
            $"bi: {entity.block?.index}",
        ];
    }
}
