using SkiaSharp;
using System;
using yoksdotnet.common;
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

        if (skin.style.IsClassic)
        {
            DrawClassic(canvas, entity, skin);
        }
        else if (skin.style.IsRefined)
        {
            DrawRefined(canvas, entity, skin);
        }

        if (entity.Get<Bubble>() is { } bubble)
        {
            BubblePainter.Draw(canvas, entity.basis, skin, bubble);
        }
    }

    private static void DrawClassic(SKCanvas canvas, Entity entity, Skin skin)
    {
        var skBitmap = SpriteBitmaps.GetClassicBitmap(skin, entity.Get<Emotion>()).Resource;
        canvas.DrawBitmap(skBitmap, GetRect(entity), skin.BodyPaint);
    }

    private static void DrawRefined(SKCanvas canvas, Entity entity, Skin skin)
    {
        var refinedBitmap = SpriteBitmaps.GetRefinedBitmap(skin, entity.Get<Emotion>());

        var bounds = entity.basis.Bounds;

        var eyeX = (float)(bounds.topLeft.X + (refinedBitmap.PupilCenter.X * entity.basis.scale));
        var eyeY = (float)(bounds.topLeft.Y + (refinedBitmap.PupilCenter.Y * entity.basis.scale));
        var eyeSize = (float)(refinedBitmap.PupilRange * 2.5 * entity.basis.scale * refinedBitmap.EyeScale);

        var pupilX = eyeX;
        var pupilY = eyeY;

        var isBlind = skin.palette == PredefinedPalette.Loxxe;

        if (entity.Get<Gaze>() is { } gaze && !isBlind)
        {
            var eyeOrigin = new Vector(eyeX, eyeY);
            var eyeline = gaze.currentGazePoint.Sub(eyeOrigin);

            var offsetCoefficient = Interp.Linear(
                Math.Atan(eyeline.Magnitude / 1000), 
                -Math.PI / 2.0, 
                Math.PI / 2.0, 
                0.0, 
                refinedBitmap.PupilRange * entity.basis.scale
            );
            var pupilOffset = eyeline.AsNormalized().Mult(offsetCoefficient);

            pupilX += (float)pupilOffset.X;
            pupilY += (float)pupilOffset.Y;
        }

        var skBitmap = refinedBitmap.Resource;

        var pupilScaleX = 1.0f / 5.0f * (float)refinedBitmap.PupilScale;
        var pupilScaleY = 1.0f / 2.0f * (float)refinedBitmap.PupilScale;

        canvas.DrawCircle(eyeX, eyeY, eyeSize, skin.WhitesPaint);
        canvas.DrawOval(pupilX, pupilY, eyeSize * pupilScaleX, eyeSize * pupilScaleY, skin.EyePaint);
        canvas.DrawBitmap(skBitmap, GetRect(entity), skin.BodyPaint);
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
