using System;
using System.Collections.Generic;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class SpriteGenerator(ScrOptions _options, Random _rng)
{

    private readonly RandomPaletteGenerator _randomPaletteGenerator = new(new());

    private static int _runningId = 0;

    public IEnumerable<Sprite> Make(double spreadX = 1.0, double spreadY = 1.0)
    {
        var sprites = new List<Sprite>();
        var (selectedPalettes, totalPossibleCount) = SelectPalettes();

        for (var i = 0; i < _options.GetActualSpriteCount(spreadX, spreadY); i++)
        {
            var palette = selectedPalettes.SampleExponential(_rng, 1.0 - (double)selectedPalettes.Count / totalPossibleCount);
            var newSprite = new Yokin(palette)
            {
                Id = _runningId++,
                Brand = _rng.NextDouble(),
                Home = new(_rng.NextDouble() * spreadX, _rng.NextDouble() * spreadY),
                Offset = new(0, 0),
                Scale = _options.IndividualScale,
                Width = Bitmap.BitmapSize(),
                Height = Bitmap.BitmapSize(),
                AngleRadians = 0.0,
                EmotionScale = _options.GetActualEmotionScale(),
            };

            sprites.Add(newSprite);
        }

        return sprites;
    }

    private (List<Palette> Palettes, int TotalPossibleCount) SelectPalettes()
    {
        IEnumerable<Palette> possiblePalettes = _options.FamilyPaletteChoice switch
        {
            PaletteChoice.SingleGroup choice =>
                Sfes.GetAll<PredefinedPalette>()
                    .Where(pair => pair.Group == choice.Group),

            PaletteChoice.AllGroups =>
                Sfes.GetAll<PredefinedPalette>(),

            PaletteChoice.UserDefined(var setId, _) =>
                _options.CustomPalettes.FirstOrDefault(s => s.Id == setId)?.Entries
                    .Select(e => e.Palette),

            PaletteChoice.ImFeelingLucky =>
                _randomPaletteGenerator.Generate(_options.GetActualColorCount()),

            _ => throw new NotImplementedException(),
        } ?? [new Palette()];

        if (possiblePalettes.Count() == 0)
        {
            possiblePalettes = [new Palette()];
        }

        var usableColorsCount = Math.Min(_options.GetActualColorCount(), possiblePalettes.Count());

        var selectedPalettes = possiblePalettes
            .OrderBy(x => RandomUtils.SharedRng.Next())
            .Take(usableColorsCount);

        return ([..selectedPalettes], possiblePalettes.Count());
    }
}

