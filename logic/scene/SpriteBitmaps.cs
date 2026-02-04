using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public static class SpriteBitmaps
{
    public static Bitmap GetBitmap(Sprite sprite)
    {
        if (sprite.addons.fixedBitmap is not null)
        {
            return sprite.addons.fixedBitmap;
        }

        if (sprite.addons.emotions is not { } emotions)
        {
            return Bitmap.Lk;
        }

        var ambitionRanking = GetEmotionRanking(emotions.ambition);
        var empathyRanking = GetEmotionRanking(emotions.empathy);
        var optimismRanking = GetEmotionRanking(emotions.optimism);

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
            ? 0 
            : emotion < 0.66
            ? 1
            : 2;

        return rank;
    }
}
