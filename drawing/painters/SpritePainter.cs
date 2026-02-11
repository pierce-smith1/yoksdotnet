using SkiaSharp;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing.painters;

public static class SpritePainter
{
    public static void Draw(SKCanvas canvas, Entity entity)
    {
        if (entity.skin is null)
        {
            return;
        }

        if (entity.trail is not null)
        {
            for (var i = 0; i < entity.trail.snapshots.Count; i += 10)
            {
                var snapshot = entity.trail.snapshots[i];
                if (snapshot is not null)
                {
                    Draw(canvas, snapshot);
                }
            }
        }

        var skBitmap = SpriteBitmaps.GetBitmap(entity.skin, entity.emotion).Resource;
        var skPaint = entity.skin.cachedPaint ?? PaletteConversion.ToSkPaint(entity.skin.palette);

        canvas.DrawBitmap(skBitmap, GetRect(entity), skPaint);
    }

    public static SKRect GetRect(Entity entity)
    {
        var (topLeft, bottomRight) = entity.basis.Bounds;

        var rect = new SKRect((float)topLeft.X, (float)topLeft.Y, (float)bottomRight.X, (float)bottomRight.Y);
        return rect;
    }
}
