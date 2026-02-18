using SkiaSharp;
using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic;

public abstract class EntityComponent;

public class PhysicalBasis : EntityComponent
{
    public required Vector home;
    public required Vector offset;
    public required double scale;
    public required double width;
    public required double height;
    public required double angleRadians;

    public Vector Final => new(home.X + offset.X, home.Y + offset.Y);

    public double ApothemX => width * scale / 2;
    public double ApothemY => height * scale / 2;

    public (Vector topLeft, Vector bottomRight) Bounds
    {
        get
        {
            var topLeft = new Vector(Final.X - ApothemX, Final.Y - ApothemY);
            var botRight = new Vector(Final.X + ApothemX, Final.Y + ApothemY);

            return (topLeft, botRight);
        }
    }
}

public class Physics : EntityComponent
{
    public required Vector velocity;
    public required double mass;
}

public class Brand : EntityComponent
{
    public required double value;
}

public class Skin : EntityComponent
{
    public required Palette palette;
    public Bitmap? fixedBitmap = null;
    public SKPaint? cachedPaint = null;
}

public class PhysicsMeasurements : EntityComponent
{
    public required Vector lastVelocity;
}
