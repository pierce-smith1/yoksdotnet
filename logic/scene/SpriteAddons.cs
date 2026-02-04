using SkiaSharp;
using System;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class SpriteAddons
{
    public Bitmap? fixedBitmap = null;
    public EmotionVector? emotions = null;
    public SKPaint? cachedPaint = null;
}

public class EmotionVector(double ambition, double empathy, double optimism)
{
    public double ambition = ambition;
    public double empathy = empathy;
    public double optimism = optimism;

    public double Magnitude => Math.Sqrt(ambition * ambition + empathy * empathy + optimism * optimism);
}
