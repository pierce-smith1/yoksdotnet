using SkiaSharp;

using System;

namespace yoksdotnet.logic.scene;

public record struct Point(double X, double Y);
public record struct Vector(double X, double Y);

public partial class MoverState {}

public class Sprite
{
    public required int Id;
    public required double Brand;

    public required Point Home;
    public required Vector Offset;

    public required double Scale;
    public required double Width;
    public required double Height;
    public required double AngleRadians;

    public required SKPaint Paint;

    public MoverState MoverState = new();

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
