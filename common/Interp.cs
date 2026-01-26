using System;

namespace yoksdotnet.common;

public static class Interp
{
    public static double Linear(double n, double inMin, double inMax, double outMin, double outMax)
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

    public static double Power(double n, double exponent, double inMin, double inMax, double outMin, double outMax)
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

    public static double Square(double n, double inMin, double inMax, double outMin, double outMax)
    {
        return Power(n, 2, inMin, inMax, outMin, outMax);
    }

    public static double Sqrt(double n, double inMin, double inMax, double outMin, double outMax)
    {
        return Power(n, 1.0 / 2.0 , inMin, inMax, outMin, outMax);
    }
}
