using SkiaSharp;

using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing;

public class EntityPainter
{
    public void Draw(SKCanvas canvas, Sprite sprite)
    {
        var bitmap = GetBitmap(sprite);
        var paint = GetPaint(sprite);

        canvas.DrawBitmap(bitmap, sprite.GetRect(), paint);
    }

    private SKBitmap GetBitmap(Sprite sprite)
    {
        if (sprite is Yokin yokin)
        {
            return GetBitmapForYokin(yokin);
        }

        return Bitmaps.Vx;
    }

    private SKBitmap GetBitmapForYokin(Yokin yokin)
    {
        var ambitionRanking = getEmotionRanking(yokin.Emotion.Ambition) + 1;
        var empathyRanking = getEmotionRanking(yokin.Emotion.Empathy) + 1;
        var optimismRanking = getEmotionRanking(yokin.Emotion.Optimism) + 1;

        SKBitmap[][][] emotionMap =
        [
            [
                [Bitmaps.LkSix, Bitmaps.LkHusk, Bitmaps.LkHusk],
                [Bitmaps.LkUnamused, Bitmaps.LkUnamused, Bitmaps.LkSix],
                [Bitmaps.LkXd, Bitmaps.LkXd, Bitmaps.LkSix],
            ],
            [
                [Bitmaps.LkUnamused, Bitmaps.LkUnamused, Bitmaps.Lk],
                [Bitmaps.LkConcern, Bitmaps.Lk, Bitmaps.Lk],
                [Bitmaps.LkConcern, Bitmaps.LkThumbsup, Bitmaps.LkThumbsup],
            ],
            [
                [Bitmaps.LkExhausted, Bitmaps.LkExhausted, Bitmaps.LkExhausted],
                [Bitmaps.LkThink, Bitmaps.LkThink, Bitmaps.LkJoy],
                [Bitmaps.LkJoy, Bitmaps.LkCool, Bitmaps.LkSix],
            ],
        ];

        var result = emotionMap[empathyRanking][optimismRanking][ambitionRanking];
        return result;

        static int getEmotionRanking(double emotion)
        {
            if (emotion < 0.33) return -1;
            if (emotion < 0.66) return 0;
            return 1;
        }
    }

    private SKPaint GetPaint(Sprite sprite)
    {
        return sprite.Paint;
    }
}
