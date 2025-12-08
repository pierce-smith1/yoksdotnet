using System;

using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public record struct Point(double X, double Y);
public record struct Vector(double X, double Y);

public partial class MoverState {}

public abstract class Sprite
{
    public required int Id;
    public required double Brand;

    public required Point Home;
    public required Vector Offset;

    public required double Scale;
    public required double Width;
    public required double Height;
    public required double AngleRadians;

    public MoverState MoverState = new();

    public abstract Bitmap GetBitmap();
    public abstract Palette GetPalette();

    public Point GetFinalPos()
    {
        return new(Home.X + Offset.X, Home.Y + Offset.Y);
    }

    public (Point TopLeft, Point BotRight) GetBounds()
    {
        var final = GetFinalPos();

        var topLeft = final;
        var botRight = new Point(final.X + (Width * Scale), final.Y + (Height * Scale));

        return (topLeft, botRight);
    }
}

public class Yokin : Sprite
{
    public record struct EmotionVector(double Empathy, double Ambition, double Optimism);

    public required double EmotionScale { get; set; }
    private EmotionVector Emotion = new();

    public required Palette Palette { get; set; }

    public override Palette GetPalette() => Palette;

    public override Bitmap GetBitmap()
    {
        var ambitionRanking = getEmotionRanking(GetEmotionVector().Ambition) + 1;
        var empathyRanking = getEmotionRanking(GetEmotionVector().Empathy) + 1;
        var optimismRanking = getEmotionRanking(GetEmotionVector().Optimism) + 1;

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

        static int getEmotionRanking(double emotion)
        {
            if (emotion < -0.66) return -1;
            if (emotion < 0.66) return 0;
            return 1;
        }
    }

    public EmotionVector GetEmotionVector()
    {
        return new(Emotion.Empathy * EmotionScale, Emotion.Ambition * EmotionScale, Emotion.Optimism * EmotionScale);
    }

    public void SetEmotionVector(double empathy, double ambition, double optimism)
    {
        Emotion.Empathy = empathy;
        Emotion.Ambition = ambition;
        Emotion.Optimism = optimism;
    }

    public double GetEmotionStrength()
    {
        var (e, a, o) = GetEmotionVector();

        var strength =  Math.Sqrt(e * e + a * a + o * o);
        return strength;
    }
}
