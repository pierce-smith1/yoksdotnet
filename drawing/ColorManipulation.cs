using System;
using yoksdotnet.common;

namespace yoksdotnet.drawing;

public class ColorManipulation
{
    public static RgbColor DarkenColor(RgbColor color)
    {
        var (h, s, l) = ColorConversion.ToHsl(color);

        h = ShiftHueTowards(hue: h, target: 240.0, factor: 1.2);
        l /= 2;

        var newColor = ColorConversion.FromHsl(new(h, s, l));
        return newColor;
    }

    public static RgbColor LightenColor(RgbColor color)
    {
        var (h, s, l) = ColorConversion.ToHsl(color);

        h = ShiftHueTowards(hue: h, target: 50.0, factor: 1.2);
        l = Interp.Linear(l, 0.0, 100.0, 50.0, 100.0);

        var newColor = ColorConversion.FromHsl(new(h, s, l));
        return newColor;
    }

    public static RgbColor WhitenColor(RgbColor color)
    {
        var (h, s, l) = ColorConversion.ToHsl(color);

        l = Interp.Linear(l, 0.0, 100.0, 90.0, 100.0);

        var newColor = ColorConversion.FromHsl(new(h, s, l));
        return newColor;
    }

    public static RgbColor ColorBetween(RgbColor a, RgbColor b)
    {
        var avgR = (byte)((a.R + b.R) / 2);
        var avgG = (byte)((a.G + b.G) / 2);
        var avgB = (byte)((a.B + b.B) / 2);

        return new(avgR, avgG, avgB);
    }

    public static double ShiftHueTowards(double hue, double target, double factor)
    {
        hue -= target;

        while (hue < 180)
        {
            hue += 360;
        }

        while (hue > 180)
        {
            hue -= 360;
        }

        hue = Interp.Linear(hue, -target, 360 - target, -target / factor, (360 - target) / factor);
        hue += target;

        return hue;
    }

    public static int Distance(RgbColor a, RgbColor b)
    {
        var dr = Math.Abs(a.R - b.R);
        var dg = Math.Abs(a.G - b.G);
        var db = Math.Abs(a.B - b.B);

        return dr + dg + db;
    }
}
