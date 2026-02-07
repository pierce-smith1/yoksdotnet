using System;
using System.Collections.Generic;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class SpriteGenerator(Random rng, ScrOptions options)
{
    private static int _runningId = 0;

    public IEnumerable<Sprite> Make(double spreadX, double spreadY)
    {
        var sprites = new List<Sprite>();
        var (selectedPalettes, totalPossibleCount) = SelectPalettes();

        for (var i = 0; i < GetSpriteCount(spreadX, spreadY); i++)
        {
            var palette = rng.SampleExponential(selectedPalettes, 1.0 - (double)selectedPalettes.Count / totalPossibleCount);
            palette ??= Palette.DefaultPalette;

            var newSprite = new Sprite
            {
                id = _runningId++,
                brand = rng.NextDouble(),
                palette = palette,
                home = new(rng.NextDouble() * spreadX, rng.NextDouble() * spreadY),
                offset = new(0, 0),
                scale = options.spriteScale,
                width = Bitmap.BitmapSize(),
                height = Bitmap.BitmapSize(),
                angleRadians = 0.0,

                addons = new SpriteAddons
                {
                    emotions = new(0.0, 0.0, 0.0),
                    cachedPaint = PaletteConversion.ToSkPaint(palette),
                }
            };

            sprites.Add(newSprite);
        }

        return sprites;
    }

    private (List<Palette> Palettes, int TotalPossibleCount) SelectPalettes()
    {
        IEnumerable<Palette> possiblePalettes = options.paletteChoice.Match(
            whenSingleGroup: group =>
                SfEnums.GetAll<PredefinedPalette>().Where(pair => pair.Group == group),

            whenAll: SfEnums.GetAll<PredefinedPalette>,

            whenUserDefined: customSet =>
                options.customPalettes.FirstOrDefault(s => s.Id == customSet.Id)?.Entries.Select(e => e.Palette),

            whenGenerated: () =>
                new RandomPaletteGenerator(rng).Generate(GetColorCount())
        ) ?? [Palette.DefaultPalette];

        if (!possiblePalettes.Any())
        {
            possiblePalettes = [Palette.DefaultPalette];
        }

        var usableColorsCount = Math.Min(GetColorCount(), possiblePalettes.Count());

        var selectedPalettes = possiblePalettes
            .OrderBy(x => rng.Next())
            .Take(usableColorsCount);

        return ([..selectedPalettes], possiblePalettes.Count());
    }

    private int GetSpriteCount(double width, double height)
    {
        var scalingFactor = Interp.Linear(options.familySize, 0.0, 1.0, 0.2, 1.0);

        var count = (width / 64) * (height / 64) * scalingFactor;
        return (int)count;
    }

    private int GetColorCount()
    {
        var maxColorCount = options.paletteChoice.Match(
            whenSingleGroup: group =>
                SfEnums.GetAll<PredefinedPalette>().Where(p => p.Group == group).Count(),

            whenAll: () =>
                SfEnums.GetAll<PredefinedPalette>().Count(),

            whenUserDefined: customSet
                => options.customPalettes.FirstOrDefault(s => s.Id == customSet.Id)?.Entries.Count ?? 1,

            whenGenerated: () => 30
        );

        var count = Math.Max(2, (int)Math.Round(options.diversity * maxColorCount));
        return count;
    }
}

