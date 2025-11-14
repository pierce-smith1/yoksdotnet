using System;

namespace yoksdotnet.common;

public static class Interpolation
{
    public static double InterpLinear(double n, double inMin, double inMax, double outMin, double outMax)
    {
        if (n < inMin)
        {
            return outMin;
        }

        if (n > inMax)
        {
            return outMax;
        }

        var result = (outMax - outMin) / (inMax - inMin) * (n - inMin) + outMin;
        return result;
    }

    public static double InterpPower(double n, double exponent, double inMin, double inMax, double outMin, double outMax)
    {
        if (n < inMin)
        {
            return outMin;
        }

        if (n > inMax)
        {
            return outMax;
        }

        var result = Math.Pow(Math.Pow(outMax - outMin, 1 / exponent) / (inMax - inMin) * (n - inMin), exponent) + outMin;
        return result;
    }

    public static double InterpSquare(double n, double inMin, double inMax, double outMin, double outMax)
    {
        return InterpPower(n, 2, inMin, inMax, outMin, outMax);
    }

    public static double InterpSqrt(double n, double inMin, double inMax, double outMin, double outMax)
    {
        return InterpPower(n, 1 / 2 , inMin, inMax, outMin, outMax);
    }
}
