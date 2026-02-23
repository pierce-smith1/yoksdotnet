using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public static class SpriteBitmaps
{
    public static ClassicBitmap GetClassicBitmap(Skin skin, Emotion? emotion)
    {
        if (skin.fixedBitmap?.ClassicBitmap is { } bitmap)
        {
            return bitmap;
        }

        if (emotion is null)
        {
            return ClassicBitmap.Lk;
        }

        var ambitionRanking = GetEmotionRanking(emotion.ambition);
        var empathyRanking = GetEmotionRanking(emotion.empathy);
        var optimismRanking = GetEmotionRanking(emotion.optimism);

        ClassicBitmap[][][] emotionMap =
        [
            [
                [ClassicBitmap.LkSix, ClassicBitmap.LkHusk, ClassicBitmap.LkHusk],
                [ClassicBitmap.LkUnamused, ClassicBitmap.LkUnamused, ClassicBitmap.LkSix],
                [ClassicBitmap.LkXd, ClassicBitmap.LkXd, ClassicBitmap.LkSix],
            ],
            [
                [ClassicBitmap.LkUnamused, ClassicBitmap.LkUnamused, ClassicBitmap.Lk],
                [ClassicBitmap.LkConcern, ClassicBitmap.Lk, ClassicBitmap.Lk],
                [ClassicBitmap.LkConcern, ClassicBitmap.LkThumbsup, ClassicBitmap.LkThumbsup],
            ],
            [
                [ClassicBitmap.LkExhausted, ClassicBitmap.LkExhausted, ClassicBitmap.LkExhausted],
                [ClassicBitmap.LkThink, ClassicBitmap.LkThink, ClassicBitmap.LkJoy],
                [ClassicBitmap.LkJoy, ClassicBitmap.LkCool, ClassicBitmap.LkSix],
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

    public static RefinedBitmap GetRefinedBitmap(Skin skin, Emotion? emotion)
    {
        if (skin.fixedBitmap?.RefinedBitmap is { } bitmap)
        {
            return bitmap;
        }

        if (emotion is null)
        {
            return RefinedBitmap.Neutral;
        }

        var ambitionRanking = GetEmotionRanking(emotion.ambition);
        var empathyRanking = GetEmotionRanking(emotion.empathy);
        var optimismRanking = GetEmotionRanking(emotion.optimism);

        RefinedBitmap[][][] emotionMap =
        [
            [
                [RefinedBitmap.Unamused, RefinedBitmap.Unamused, RefinedBitmap.Cry],
                [RefinedBitmap.Unamused, RefinedBitmap.Neutral, RefinedBitmap.Cry],
                [RefinedBitmap.Angery, RefinedBitmap.Angery, RefinedBitmap.Cry],
            ],
            [
                [RefinedBitmap.Unamused, RefinedBitmap.Neutral, RefinedBitmap.Joy],
                [RefinedBitmap.Neutral, RefinedBitmap.Neutral, RefinedBitmap.Neutral],
                [RefinedBitmap.Angery, RefinedBitmap.Neutral, RefinedBitmap.Cry],
            ],
            [
                [RefinedBitmap.Evil, RefinedBitmap.Joy, RefinedBitmap.Joy],
                [RefinedBitmap.Evil, RefinedBitmap.Neutral, RefinedBitmap.Scream],
                [RefinedBitmap.Evil, RefinedBitmap.Scream, RefinedBitmap.Scream],
            ],
        ];

        var result = emotionMap[empathyRanking][optimismRanking][ambitionRanking];
        return result;
    }
}
