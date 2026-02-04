using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using yoksdotnet.common;
using yoksdotnet.logic.scene;

namespace yoksdotnet.drawing.painters;

using PaletteIndexRegions = Dictionary<PaletteIndex, List<Point>>;

public class SpriteEditPreviewPainter(Bitmap bitmap)
{
    private readonly PaletteIndexRegions _paletteIndexRegions = ComputeIndexRegions(bitmap.Resource);
    private readonly Sprite _previewSprite = new()
    {
        palette = Palette.DefaultPalette,
    };

    public PaletteIndex? HoveredIndex { get; private set; } = null;

    public void Draw(SKCanvas canvas, Palette? palette)
    {
        _previewSprite.home.X = (canvas.LocalClipBounds.Width / 2) - (Bitmap.BitmapSize() / 2);
        _previewSprite.home.Y = (canvas.LocalClipBounds.Height / 2) - (Bitmap.BitmapSize() / 2);

        _previewSprite.palette = palette is not null
            ? HoveredIndex is not null
                ? WithIndexHighlighted(palette, HoveredIndex)
                : palette
            : Palette.DefaultPalette;

        canvas.Clear(new SKColor(0x11, 0x11, 0x11));

        SpritePainter.Draw(canvas, _previewSprite);
    }

    public void UpdateHoveredIndex(float x, float y, float width, float height)
    {
        var hoverPos = new Point(x, y);

        foreach (var (index, points) in _paletteIndexRegions)
        {
            var offsetPoints = points.Select(p => new Point(
                p.X + width / 2.0 - Bitmap.BitmapSize() / 2.0,
                p.Y + height / 2.0 - Bitmap.BitmapSize() / 2.0
            ));

            if (offsetPoints.Contains(hoverPos))
            {
                HoveredIndex = index;
                return;
            }
        }

        HoveredIndex = null;
    }

    private static Palette WithIndexHighlighted(Palette palette, PaletteIndex index)
    {
        var newPalette = PaletteConverter.Copy(palette);

        var prevColor = newPalette[index];

        const int lightenAmount = 100;

        var newColor = new RgbColor(
            (byte)Math.Clamp(prevColor.R + lightenAmount, 0, 255),
            (byte)Math.Clamp(prevColor.G + lightenAmount, 0, 255),
            (byte)Math.Clamp(prevColor.B + lightenAmount, 0, 255)
        );

        newPalette[index] = newColor;

        return newPalette;
    }

    private static PaletteIndexRegions ComputeIndexRegions(SKBitmap bitmap)
    {
        var indexes = SfEnums.GetAll<PaletteIndex>();

        var regions = new PaletteIndexRegions();

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (color.Alpha != 255)
                {
                    continue;
                }

                var index = indexes.FirstOrDefault(i => i.Luminance == color.Red);
                if (index is null)
                {
                    continue;
                }

                regions.TryAdd(index, []);
                regions[index].Add(new(x, y));
            }
        }

        return regions;
    }
}
