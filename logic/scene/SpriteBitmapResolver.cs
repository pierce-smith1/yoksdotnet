using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public static class SpriteBitmapResolver
{
    public static Bitmap GetBitmap(Sprite sprite)
    {
        if (sprite.fixedBitmap is not null)
        {
            return sprite.fixedBitmap;
        }

        if (sprite.emotions is not { } emotions)
        {
            return Bitmap.Lk;
        }

        var ambitionRanking = GetEmotionRanking(emotions.Ambition) + 1;
        var empathyRanking = GetEmotionRanking(emotions.Empathy) + 1;
        var optimismRanking = GetEmotionRanking(emotions.Optimism) + 1;

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
        return result;
    }

    private static int GetEmotionRanking(double emotion)
    {
        var rank = emotion < -0.66
            ? -1
            : emotion < 0.66
            ? 0
            : 1;

        return rank;
    }
}
