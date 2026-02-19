using System;

namespace yoksdotnet.common;

public record struct Vector(double X, double Y)
{
    public readonly double Magnitude => DistanceTo(new(0.0, 0.0));

    public static Vector RandomScaled(Random rng, double scaleX, double scaleY)
    {
        var x = Interp.Linear(rng.NextDouble(), 0.0, 1.0, -1.0, 1.0) * scaleX;
        var y = Interp.Linear(rng.NextDouble(), 0.0, 1.0, -1.0, 1.0) * scaleY;

        return new(x, y);
    }

    public Vector Mult(double scale)
    {
        X *= scale;
        Y *= scale;
        return this;
    }

    public readonly Vector Plus(Vector other)
    {
        var result = new Vector(X + other.X, Y + other.Y);
        return result;
    }

    public readonly Vector Sub(Vector other)
    {
        var result = new Vector(X - other.X, Y - other.Y);
        return result;
    }

    public readonly double DistanceTo(Vector other)
    {
        var distance = Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        return distance;
    }

    public readonly double Dot(Vector other)
    {
        var result = X * other.X + Y * other.Y;
        return result;
    }

    public Vector Normalize()
    {
        var magnitude = Magnitude;
        if (magnitude == 0.0)
        {
            return this;
        }

        X *= 1.0 / magnitude;
        Y *= 1.0 / magnitude;
        return this;
    }

    public Vector AsNormalized()
    {
        var result = new Vector(X, Y).Normalize();
        return result;
    }
}

