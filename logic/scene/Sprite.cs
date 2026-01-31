using System;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class Sprite
{
    public required int id;
    public required double brand;
    public required Palette palette;

    public Point home = new(0.0, 0.0);
    public Point offset = new(0.0, 0.0);

    public double scale = 1.0;
    public double width = Bitmap.BitmapSize();
    public double height = Bitmap.BitmapSize();
    public double angleRadians = 0.0;

    public SpriteAddons addons = new();

    public Point FinalPos => new(home.X + offset.X, home.Y + offset.Y);
    public (Point topLeft, Point bottomRight) Bounds
    {
        get
        {
            var topLeft = FinalPos;
            var botRight = new Point(FinalPos.X + (width * scale), FinalPos.Y + (height * scale));

            return (topLeft, botRight);
        }
    }
}

public record struct Point(double X, double Y);
