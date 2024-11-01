using System;

using yoksdotnet.common;

namespace yoksdotnet.logic;

public static class PerlinNoise
{
    private readonly ref struct NoiseVector
    {
        public required double X { get; init; }
        public required double Y { get; init; }
        public required double Z { get; init; }

        public NoiseVector Subtract(NoiseVector other)
        {
            NoiseVector difference = new() 
            { 
                X = X - other.X,
                Y = Y - other.Y,
                Z = Z - other.Z,
            };
            return difference;
        }

        public double Dot(NoiseVector other)
        {
            var product = X * other.X + Y * other.Y + Z * other.Z;
            return product;
        }
    }

    private readonly ref struct CellVectors
    {
        public required NoiseVector A { get; init; }
        public required NoiseVector B { get; init; }
        public required NoiseVector C { get; init; }
        public required NoiseVector D { get; init; }
        public required NoiseVector E { get; init; }
        public required NoiseVector F { get; init; }
        public required NoiseVector G { get; init; }
        public required NoiseVector H { get; init; }
    }

    private readonly ref struct CellDots
    {
        public required double A { get; init; }
        public required double B { get; init; }
        public required double C { get; init; }
        public required double D { get; init; }
        public required double E { get; init; }
        public required double F { get; init; }
        public required double G { get; init; }
        public required double H { get; init; }
    }

    private static readonly RandomUtils.FastRandom _rng = new();
    private static readonly uint _baseSeed = (uint)DateTimeOffset.Now.Millisecond;

    public static double Get(double x, double y, double z)
    {
        NoiseVector v = new() { X = x, Y = y, Z = z };

        var dots = GetCellDots(v);
        var result = CellInterpolate(ref dots, v);
        return result;
    }

    private static NoiseVector GetGradVector(double x, double y, double z)
    {
        double seed = _baseSeed;
        seed = 31 * seed + x;
        seed = 31 * seed + y;
        seed = 31 * seed + z;

        _rng.SetSeed((uint)seed);

        double fX = _rng.NextDouble();
        double fY = _rng.NextDouble();
        double fZ = _rng.NextDouble();

        double mag = Math.Sqrt(fX * fX + fY * fY + fZ * fZ);
        fX /= mag;
        fY /= mag;
        fZ /= mag;

        return new() { X = fX, Y = fY, Z = fZ };
    }

    private static NoiseVector GetGradVectorFromCorner(NoiseVector v)
    {
        var gradVector = GetGradVector(Math.Round(v.X), Math.Round(v.Y), Math.Round(v.Z));
        return gradVector;
    }

    private static CellVectors GetCellCorners(NoiseVector v)
    {
        double x = Math.Floor(v.X);
        double y = Math.Floor(v.Y);
        double z = Math.Floor(v.Z);

        CellVectors corners = new()
        {
            A = new() { X = x,       Y = y,       Z = z       },
            B = new() { X = x + 1.0, Y = y,       Z = z       },
            C = new() { X = x,       Y = y + 1.0, Z = z       },
            D = new() { X = x,       Y = y,       Z = z + 1.0 },
            E = new() { X = x + 1.0, Y = y + 1.0, Z = z       },
            F = new() { X = x + 1.0, Y = y,       Z = z + 1.0 },
            G = new() { X = x,       Y = y + 1.0, Z = z + 1.0 },
            H = new() { X = x + 1.0, Y = y + 1.0, Z = z + 1.0 },
        };
        return corners;
    }

    private static CellVectors GetOffsetVectors(NoiseVector v)
    {
        var corners = GetCellCorners(v);

        CellVectors offsets = new()
        {
            A = v.Subtract(corners.A),
            B = v.Subtract(corners.B),
            C = v.Subtract(corners.C),
            D = v.Subtract(corners.D),
            E = v.Subtract(corners.E),
            F = v.Subtract(corners.F),
            G = v.Subtract(corners.G),
            H = v.Subtract(corners.H)
        };
        return offsets;
    }

    private static CellDots GetCellDots(NoiseVector v)
    {
        var offsets = GetOffsetVectors(v);
        var corners = GetCellCorners(v);

        CellDots dots = new()
        {
            A = offsets.A.Dot(GetGradVectorFromCorner(corners.A)),
            B = offsets.B.Dot(GetGradVectorFromCorner(corners.B)),
            C = offsets.C.Dot(GetGradVectorFromCorner(corners.C)),
            D = offsets.D.Dot(GetGradVectorFromCorner(corners.D)),
            E = offsets.E.Dot(GetGradVectorFromCorner(corners.E)),
            F = offsets.F.Dot(GetGradVectorFromCorner(corners.F)),
            G = offsets.G.Dot(GetGradVectorFromCorner(corners.G)),
            H = offsets.H.Dot(GetGradVectorFromCorner(corners.H)),
        };
        return dots;
    }

    private static double CellInterpolate(ref readonly CellDots dots, NoiseVector v)
    {
        double wX = v.X - Math.Floor(v.X);
        double wY = v.Y - Math.Floor(v.Y);
        double wZ = v.Z - Math.Floor(v.Z);

        double bottomI1 = Interpolate(dots.A, dots.B, wX);
        double bottomI2 = Interpolate(dots.C, dots.E, wX);
        double bottom = Interpolate(bottomI1, bottomI2, wY);

        double topI1 = Interpolate(dots.C, dots.E, wX);
        double topI2 = Interpolate(dots.G, dots.H, wX);
        double top = Interpolate(topI1, topI2, wY);

        double result = Interpolate(bottom, top, wZ);
        return result;
    }

    private static double Interpolate(double a, double b, double w)
    {
        var result = (b - a) * (3.0 - w * 2.0) * w * w + a;
        return result;
    }
}