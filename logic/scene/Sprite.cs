using System;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class Sprite
{
    public int id = 0;
    public double brand = 0.0;
    public Palette palette = Palette.DefaultPalette;

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

    public Bitmap CurrentBitmap => SpriteBitmapResolver.GetBitmap(this);
}

public record struct Point(double X, double Y);
