using SkiaSharp;
using yoksdotnet.data;

namespace yoksdotnet.drawing.painters;

public static class PatternedBackgroundPainter
{
    private static readonly SKShader _backgroundShader =
        Bitmaps.LoadResource("/resources/backgrounds/runed.png")
            .ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);

    public static void Draw(SKCanvas canvas, AnimationContext ctx)
    {
        var matrix = SKMatrix.CreateRotationDegrees(15.0f);
        matrix.TransX = (float)ctx.scene.seconds * 200.0f;
        matrix.TransY = (float)ctx.scene.seconds * 140.0f;

        // TODO: This is maybe horrible on memory??
        using var paint = new SKPaint()
        {
            Shader = _backgroundShader.WithLocalMatrix(matrix)
        };

        canvas.DrawRect(
            0.0f, 
            0.0f,
            ctx.scene.width, 
            ctx.scene.height, 
            paint
        );
    }
}
