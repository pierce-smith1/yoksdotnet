using System;
using System.Linq;

namespace yoksdotnet.logic;

public class PerlinNoiseGenerator
{
    private readonly Random _rng;
    private readonly int[] _permutation;

    public PerlinNoiseGenerator(Random rng)
    {
        _rng = rng;
        _permutation = Enumerable.Range(0, 256).ToArray();
        _rng.Shuffle(_permutation);
        _permutation = [.._permutation, .._permutation];
    }

    // Adapted from https://mrl.cs.nyu.edu/~perlin/noise/
    public double Get(double x, double y, double z)
    {
        var cubeX = (int)Math.Floor(x) & 255;
        var cubeY = (int)Math.Floor(y) & 255;
        var cubeZ = (int)Math.Floor(z) & 255;

        var relativeX = x - Math.Floor(x);
        var relativeY = y - Math.Floor(y);
        var relativeZ = z - Math.Floor(z);

        var u = Fade(relativeX);
        var v = Fade(relativeY);
        var w = Fade(relativeZ);

        var cornerHashA = _permutation[cubeX] + cubeY;
        var cornerHashAA = _permutation[cornerHashA] + cubeZ;
        var cornerHashAB = _permutation[cornerHashA + 1] + cubeZ;
        var cornerHashB = _permutation[cubeX + 1] + cubeY;
        var cornerHashBA = _permutation[cornerHashB] + cubeZ;
        var cornerHashBB = _permutation[cornerHashB + 1] + cubeZ;

        var cornerGradAA  = Grad(_permutation[cornerHashAA],     relativeX,     relativeY,     relativeZ    );
        var cornerGradBA  = Grad(_permutation[cornerHashBA],     relativeX - 1, relativeY,     relativeZ    );
        var cornerGradAB  = Grad(_permutation[cornerHashAB],     relativeX,     relativeY - 1, relativeZ    );
        var cornerGradBB  = Grad(_permutation[cornerHashBB],     relativeX - 1, relativeY - 1, relativeZ    );
        var cornerGradAAZ = Grad(_permutation[cornerHashAA + 1], relativeX,     relativeY,     relativeZ - 1);
        var cornerGradBAZ = Grad(_permutation[cornerHashBA + 1], relativeX - 1, relativeY,     relativeZ - 1);
        var cornerGradABZ = Grad(_permutation[cornerHashAB + 1], relativeX,     relativeY - 1, relativeZ - 1);
        var cornerGradBBZ = Grad(_permutation[cornerHashBB + 1], relativeX - 1, relativeY - 1, relativeZ - 1);

        var lerpXA = Lerp(u, cornerGradAA, cornerGradBA);
        var lerpXB = Lerp(u, cornerGradAB, cornerGradBB);
        var lerpXC = Lerp(u, cornerGradAAZ, cornerGradBAZ);
        var lerpXD = Lerp(u, cornerGradABZ, cornerGradBBZ);

        var lerpYA = Lerp(v, lerpXA, lerpXB);
        var lerpYB = Lerp(v, lerpXC, lerpXD);

        var lerpZ = Lerp(w, lerpYA, lerpYB);
        
        var result = (lerpZ + 1) / 2;
        return result;
    }

    private double Fade(double t)
    {
        var result = t * t * t * (t * (t * 6 - 15) + 10);
        return result;
    }

    private double Lerp(double t, double a, double b)
    {
        var result = a + t * (b - a);
        return result;
    }

    private double Grad(int hash, double x, double y, double z)
    {
        int direction = hash & 15;
        
        double u = direction < 8 ? x : y;
        double v = direction < 4 ? y : direction == 12 || direction == 14 ? x : z;

        var result = ((direction & 1) == 0 ? u : -u) + ((direction & 2) == 0 ? v : -v);
        return result;
    }
};
