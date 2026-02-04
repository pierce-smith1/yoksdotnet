using System;
using System.Collections.Generic;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class SpriteGenerator(ScrOptions options, Random rng)
{
    private readonly RandomPaletteGenerator _randomPaletteGenerator = new(rng);
    private readonly RandomSampler _sampler = new(rng);

    private int _runningId = 0;

    public IEnumerable<Sprite> Make(double spreadX, double spreadY)
    {
        var sprites = new List<Sprite>();
        var (selectedPalettes, totalPossibleCount) = SelectPalettes();

        for (var i = 0; i < GetSpriteCount(spreadX, spreadY); i++)
        {
            var palette = _sampler.SampleExponential(selectedPalettes, 1.0 - (double)selectedPalettes.Count / totalPossibleCount);
            palette ??= Palette.DefaultPalette;

            var newSprite = new Sprite
            {
                id = _runningId++,
                brand = rng.NextDouble(),
                palette = palette,
                home = new(rng.NextDouble() * spreadX, rng.NextDouble() * spreadY),
                offset = new(0, 0),
                scale = options.IndividualScale,
                width = Bitmap.BitmapSize(),
                height = Bitmap.BitmapSize(),
                angleRadians = 0.0,

                addons = new SpriteAddons
                {
                    emotions = new(0.0, 0.0, 0.0),
                    cachedPaint = PaletteConverter.ToSkPaint(palette),
                }
            };

            sprites.Add(newSprite);
        }

        return sprites;
    }

    private (List<Palette> Palettes, int TotalPossibleCount) SelectPalettes()
    {
        IEnumerable<Palette> possiblePalettes = options.FamilyPaletteChoice switch
        {
            PaletteChoice.SingleGroup choice =>
                SfEnums.GetAll<PredefinedPalette>().Where(pair => pair.Group == choice.Group),

            PaletteChoice.AllGroups =>
                SfEnums.GetAll<PredefinedPalette>(),

            PaletteChoice.UserDefined(var setId, _) =>
                options.CustomPalettes.FirstOrDefault(s => s.Id == setId)?.Entries.Select(e => e.Palette),

            PaletteChoice.ImFeelingLucky =>
                _randomPaletteGenerator.Generate(GetColorCount()),

            _ => throw new NotImplementedException(),
        } ?? [Palette.DefaultPalette];

        if (possiblePalettes.Count() == 0)
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
        var scalingFactor = Interp.Linear(options.FamilySize, 0.0, 1.0, 0.2, 1.0);

        var count = (width / 64) * (height / 64) * scalingFactor;
        return (int)count;
    }

    private int GetColorCount()
    {
        var maxColorCount = options.FamilyPaletteChoice switch
        {
            PaletteChoice.SingleGroup(PaletteGroup g) =>
                SfEnums.GetAll<PredefinedPalette>().Where(p => p.Group == g).Count(),

            PaletteChoice.AllGroups => SfEnums.GetAll<PredefinedPalette>().Count(),
            PaletteChoice.ImFeelingLucky => 30,

            PaletteChoice.UserDefined(var setId, _) =>
                options.CustomPalettes.FirstOrDefault(s => s.Id == setId)?.Entries.Count ?? 1,

            _ => throw new NotImplementedException(),
        };

        var count = Math.Max(2, (int)Math.Round(options.FamilyDiversity * maxColorCount));
        return count;
    }
}

