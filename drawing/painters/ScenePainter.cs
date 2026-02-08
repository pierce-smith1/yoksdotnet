using SkiaSharp;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;
using yoksdotnet.windows;

namespace yoksdotnet.drawing.painters;

public class ScenePainter(AnimationContext ctx, DisplayMode displayMode)
{
    public Entity? DebuggedEntity { get; set; } = null;

    public void Draw(SKCanvas canvas)
    {
        canvas.Clear(new SKColor(0x11, 0x11, 0x11));

        foreach (var entity in ctx.scene.entities)
        {
            SpritePainter.Draw(canvas, entity);
        }

        if (displayMode.IsDebug && DebuggedEntity is not null)
        {
            DebugPainter.DrawDebugInfo(canvas, DebuggedEntity);
        }
    }
}
