using SkiaSharp;
using yoksdotnet.drawing;

namespace yoksdotnet.logic;

public abstract class EntityComponent;

public class PhysicalBasis : EntityComponent
{
    public required Point home;
    public required Point offset;
    public required double scale;
    public required double width;
    public required double height;
    public required double angleRadians;

    public Point Final => new(home.X + offset.X, home.Y + offset.Y);
    public (Point topLeft, Point bottomRight) Bounds
    {
        get
        {
            var topLeft = Final;
            var botRight = new Point(Final.X + (width * scale), Final.Y + (height * scale));

            return (topLeft, botRight);
        }
    }
}

public class Physics : EntityComponent
{
    public required Point velocity;
    public required Point acceleration;
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
    public required Point lastVelocity;
}

public record struct Point(double X, double Y);
