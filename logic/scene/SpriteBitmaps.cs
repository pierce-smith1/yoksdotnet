using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public static class SpriteBitmaps
{
    public static Bitmap GetBitmap(Skin skin, Emotion? emotion)
    {
        if (skin.fixedBitmap is not null)
        {
            return skin.fixedBitmap;
        }

        if (emotion is null)
        {
            return Bitmap.Lk;
        }

        var ambitionRanking = GetEmotionRanking(emotion.ambition);
        var empathyRanking = GetEmotionRanking(emotion.empathy);
        var optimismRanking = GetEmotionRanking(emotion.optimism);

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
