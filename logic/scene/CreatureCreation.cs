using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.data.entities;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class YokinCreationParameters
{
    public required Vector home;
    public required double scale;
    public required double brand;
    public required SpriteStyleChoice style;
    public required Palette palette;
    public int? trailLength = null;

    public required PaintCacheBuilder paintCache;
}

public static class CreatureCreation
{
    public static readonly double CreatureSize = 128;

    public static Entity NewDefault()
    {
        var entity = new Entity
        {
            basis = new()
            {
                home = new(0.0, 0.0),
                offset = new(0.0, 0.0),
                scale = 1.0,
                width = CreatureSize,
                height = CreatureSize,
                angleRadians = 0.0,
            },

            brand = 0.0,
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
                width = CreatureSize,
                height = CreatureSize,
                angleRadians = 0.0,
            },

            brand = parameters.brand,

            skin = new()
            {
                palette = parameters.palette,
                style = parameters.style,

                bodyPaintHandle = parameters.paintCache.GetBodyPaintHandle(parameters.palette),
                whitePaintHandle = parameters.paintCache.GetSolidColorPaintHandle(parameters.palette.whites),
                eyePaintHandle = parameters.paintCache.GetSolidColorPaintHandle(parameters.palette.eyes),

                paintCache = parameters.paintCache.Cache,
            },

            emotion = new()
            {
                ambition = 0.0,
                empathy = 0.0,
                optimism = 0.0,
            },
        };

        if (parameters.trailLength is not null)
        {
            yokin.trail = new Trail()
            {
                snapshots = new CircularBuffer<Entity?>(parameters.trailLength.Value * 10),
            };
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
                width = CreatureSize,
                height = CreatureSize,
                angleRadians = 0.0,
            },

            brand = 0.0,

            skin = new()
            {
                palette = Palette.DefaultPalette,
                style = bitmap.ClassicBitmap is not null ? SpriteStyleChoice.Classic() : SpriteStyleChoice.Refined(),
                fixedBitmap = bitmap,
            },

            gaze = new() 
            {
                currentGazePoint = new(0.0, 0.0),
                targetGazePoint = new(0.0, 0.0),
                targetChangeCooldownSeconds = 0.0,
            },
        };

        return yokinnequin;
    }
}
