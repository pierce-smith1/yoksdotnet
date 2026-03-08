using yoksdotnet.common;

namespace yoksdotnet.data.entities;

public class Basis : EntityComponent
{
    public required Vector home;
    public required Vector offset;
    public required double scale;
    public required double width;
    public required double height;
    public required double angleRadians;

    public Vector Final => new(home.X + offset.X, home.Y + offset.Y);

    public double ApothemX => width * scale / 2.0;
    public double ApothemY => height * scale / 2.0;

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

