using SkiaSharp;
using yoksdotnet.data;
using yoksdotnet.data.entities;
using yoksdotnet.windows;

namespace yoksdotnet.drawing.painters;

public class ScenePainter(AnimationContext ctx, DisplayMode displayMode)
{
    public Entity? DebuggedEntity { get; set; } = null;

    public void Draw(SKCanvas canvas)
    {
        DrawBackground(canvas);

        foreach (var entity in ctx.scene.entities)
        {
            SpritePainter.Draw(canvas, entity);
        }

        if (displayMode.IsDebug && DebuggedEntity is not null)
        {
            DebugPainter.DrawDebugInfo(canvas, DebuggedEntity);
        }
    }

    public void DrawBackground(SKCanvas canvas)
    {
        if (ctx.options.backgroundStyle.IsPureBlack)
        {
            canvas.Clear(SKColors.Black);
        }

        if (ctx.options.backgroundStyle.IsPatterned)
        {
            PatternedBackgroundPainter.Draw(canvas, ctx);
        }
    }
}
