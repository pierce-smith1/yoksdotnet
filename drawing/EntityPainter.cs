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

        return Bitmap.Vx.Resource;
    }

    private SKBitmap GetBitmapForYokin(Yokin yokin)
    {
        var ambitionRanking = getEmotionRanking(yokin.GetEmotionVector().Ambition) + 1;
        var empathyRanking = getEmotionRanking(yokin.GetEmotionVector().Empathy) + 1;
        var optimismRanking = getEmotionRanking(yokin.GetEmotionVector().Optimism) + 1;

        Bitmap[][][] emotionMap =
        [
            [
                [Bitmap.LkSix, Bitmap.LkHusk, Bitmap.LkHusk],
                [Bitmap.LkUnamused, Bitmap.LkUnamused, Bitmap.LkSix],
                [Bitmap.LkXd, Bitmap.LkXd, Bitmap.LkSix],
            ],
            [
                [Bitmap.LkUnamused, Bitmap.LkUnamused, Bitmap.Lk],
                [Bitmap.LkConcern, Bitmap.Lk, Bitmap.Lk],
                [Bitmap.LkConcern, Bitmap.LkThumbsup, Bitmap.LkThumbsup],
            ],
            [
                [Bitmap.LkExhausted, Bitmap.LkExhausted, Bitmap.LkExhausted],
                [Bitmap.LkThink, Bitmap.LkThink, Bitmap.LkJoy],
                [Bitmap.LkJoy, Bitmap.LkCool, Bitmap.LkSix],
            ],
        ];

        var result = emotionMap[empathyRanking][optimismRanking][ambitionRanking];
        return result.Resource;

        static int getEmotionRanking(double emotion)
        {
            if (emotion < -0.66) return -1;
            if (emotion < 0.66) return 0;
            return 1;
        }
    }

    private SKPaint GetPaint(Sprite sprite)
    {
        return sprite.Paint;
    }
}
