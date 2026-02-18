using SkiaSharp;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.drawing.painters;

public static class SpritePainter
{
    public static void Draw(SKCanvas canvas, Entity entity)
    {
        if (entity.Get<Skin>() is not { } skin)
        {
            return;
        }

        if (entity.Get<Trail>() is { } trail)
        {
            DrawTrail(canvas, trail);
        }

        var skBitmap = SpriteBitmaps.GetBitmap(skin, entity.Get<Emotion>()).Resource;
        var skPaint = skin.cachedPaint ?? PaletteConversion.ToSkPaint(skin.palette);

        canvas.DrawBitmap(skBitmap, GetRect(entity), skPaint);

        if (entity.Get<Bubble>() is { } bubble)
        {
            BubblePainter.Draw(canvas, entity.basis, skin, bubble);
        }
    }

    private static void DrawTrail(SKCanvas canvas, Trail trail)
    {
        for (var i = 0; i < trail.snapshots.Count; i += 10)
        {
            var snapshot = trail.snapshots[i];
            if (snapshot is not null)
            {
                Draw(canvas, snapshot);
            }
        }
    }

    public static SKRect GetRect(Entity entity)
    {
        var (topLeft, bottomRight) = entity.basis.Bounds;

        var rect = new SKRect((float)topLeft.X, (float)topLeft.Y, (float)bottomRight.X, (float)bottomRight.Y);
        return rect;
    }
}
