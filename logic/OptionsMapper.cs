using System;

namespace yoksdotnet.logic;

public class OptionsMapper
{
    public int MapSpriteCount(double input) => LerpToInt(input, 50, 200);
    public double MapSpriteScale(double input) => LerpToDouble(input, 0.2, 1.0);

    public int MapColorsCount(double input, int maxColors) => LerpToInt(input, 2, Math.Max(maxColors, 2));

    public double MapPatternChangeSeconds(double input) => LerpToDouble(input, 8.0, 60.0);

    private int LerpToInt(double input, int min, int max)
    {
        var scaledInput = input * (max - min) + min;
        var result = (int)Math.Round(scaledInput);
        return result;
    }

    private double LerpToDouble(double input, double min, double max)
    {
        var scaledInput = input * (max - min) + min;
        return scaledInput;
    }
}
