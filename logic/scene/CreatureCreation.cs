using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class YokinCreationParameters
{
    public required Vector home;
    public required double scale;
    public required double brand;
    public required Palette palette;
    public int? trailLength = null;
}

public static class CreatureCreation
{
    public static Entity NewDefault()
    {
        var entity = new Entity
        {
            basis = new()
            {
                home = new(0.0, 0.0),
                offset = new(0.0, 0.0),
                scale = 1.0,
                width = Bitmap.BitmapSize(),
                height = Bitmap.BitmapSize(),
                angleRadians = 0.0,
            }
        };

        return entity;
    }

    public static Entity NewYokin(YokinCreationParameters parameters)
    {
        var yokin = new Entity
        {
            basis = new()
            {
                home = parameters.home,
                offset = new(0.0, 0.0),
                scale = parameters.scale,
                width = Bitmap.BitmapSize(),
                height = Bitmap.BitmapSize(),
                angleRadians = 0.0,
            },
        };

        yokin.Attach(new Brand
        {
            value = parameters.brand,
        });

        yokin.Attach(new Skin
        {
            palette = parameters.palette,
            cachedPaint = PaletteConversion.ToSkPaint(parameters.palette),
        });

        yokin.Attach(new Emotion
        {
            ambition = 0.0,
            empathy = 0.0,
            optimism = 0.0,
        });

        if (parameters.trailLength is not null)
        {
            yokin.Attach(new Trail()
            {
                snapshots = new CircularBuffer<Entity?>(parameters.trailLength.Value * 10),
            });
        }

        return yokin;
    }

    public static Entity NewPreviewYokin(Bitmap bitmap)
    {
        var yokinnequin = new Entity
        {
            basis = new()
            {
                home = new(0.0, 0.0),
                offset = new(0.0, 0.0),
                scale = 1.0,
                width = Bitmap.BitmapSize(),
                height = Bitmap.BitmapSize(),
                angleRadians = 0.0,
            },
        };

        yokinnequin.Attach(new Skin
        {
            palette = Palette.DefaultPalette,
            fixedBitmap = bitmap,
        });

        return yokinnequin;
    }
}
