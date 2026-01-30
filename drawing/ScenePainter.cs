using SkiaSharp;

using yoksdotnet.logic.scene;
using yoksdotnet.windows;

namespace yoksdotnet.drawing;

public class ScenePainter
{
    public required Scene Scene { get; init; }
    public required DisplayWindow.DisplayMode DisplayMode { get; init; }

    public Sprite? DebuggedSprite { get; set; } = null;

    private readonly SpritePainter _spritePainter = new();
    private readonly DebugPainter _debugPainter = new();

    public void Draw(SKCanvas canvas)
    {
        canvas.Clear(new SKColor(0x11, 0x11, 0x11));

        foreach (var sprite in Scene?.Sprites ?? [])
        {
            _spritePainter.Draw(canvas, sprite);

        }

        if (DisplayMode is DisplayWindow.DisplayMode.Debug && DebuggedSprite is not null)
        {
            _debugPainter.DrawDebugInfo(canvas, DebuggedSprite);
        }
    }
}
