using SkiaSharp;

using System.Collections.Generic;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;
using yoksdotnet.logic.scene.patterns;

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

        DebugBoid(canvas, entity);
    }

    private static void DebugBoid(SKCanvas canvas, Entity entity)
    {
        if (entity.Get<Boid>() is not { } boid)
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
            $"Gcx: {entity.Get<Gaze>()?.currentGazePoint.X}",
            $"Gcy: {entity.Get<Gaze>()?.currentGazePoint.Y}",
        ];
    }
}
