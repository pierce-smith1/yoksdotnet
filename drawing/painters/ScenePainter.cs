using SkiaSharp;
using yoksdotnet.logic.scene;
using yoksdotnet.windows;

namespace yoksdotnet.drawing.painters;

public class ScenePainter(AnimationContext ctx, DisplayMode displayMode)
{
    public Sprite? DebuggedSprite { get; set; } = null;

    public void Draw(SKCanvas canvas)
    {
        canvas.Clear(new SKColor(0x11, 0x11, 0x11));

        foreach (var sprite in ctx.scene.sprites)
        {
            SpritePainter.Draw(canvas, sprite);
        }

        if (displayMode.IsDebug && DebuggedSprite is not null)
        {
            DebugPainter.DrawDebugInfo(canvas, DebuggedSprite);
        }
    }
}
