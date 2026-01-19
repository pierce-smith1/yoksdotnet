using System;
using System.Collections.Generic;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.logic.scene;

public class SpriteGenerator
{
    public required ScrOptions Options { get; init; }

    private static int _runningId = 0;

    public IEnumerable<Sprite> Make(double spreadX = 1.0, double spreadY = 1.0)
    {
        var rng = RandomUtils.SharedRng;
        var sprites = new List<Sprite>();
        var (selectedPalettes, totalPossibleCount) = SelectPalettes();

        for (var i = 0; i < Options.GetActualSpriteCount(spreadX, spreadY); i++)
        {
            var palette = rng.SampleExponential(selectedPalettes, 1 - (double)selectedPalettes.Count / totalPossibleCount);
            var newSprite = new Yokin(palette)
            {
                Id = _runningId++,
                Brand = rng.NextDouble(),
                Home = new(rng.NextDouble() * spreadX, rng.NextDouble() * spreadY),
                Offset = new(0, 0),
                Scale = Options.IndividualScale,
                Width = Bitmap.BitmapSize(),
                Height = Bitmap.BitmapSize(),
                AngleRadians = 0.0,
                EmotionScale = Options.GetActualEmotionScale(),
            };

            sprites.Add(newSprite);
        }

        return sprites;
    }

    private (List<Palette> Palettes, int TotalPossibleCount) SelectPalettes()
    {
        IEnumerable<Palette> possiblePalettes = Options.FamilyPaletteChoice switch
        {
            PaletteChoice.SingleGroup choice =>
                StaticFieldEnumerations.GetAll<PredefinedPalette>()
                    .Where(pair => pair.Group == choice.Group),

            PaletteChoice.AllGroups =>
                StaticFieldEnumerations.GetAll<PredefinedPalette>(),

            PaletteChoice.UserDefined(var setId, _) =>
                Options.CustomPalettes.FirstOrDefault(s => s.Id == setId)?.Entries
                    .Select(e => e.Palette),

            _ => throw new NotImplementedException(),
        } ?? [new Palette()];

        var usableColorsCount = Math.Min(Options.GetActualColorCount(), possiblePalettes.Count());

        var selectedPalettes = possiblePalettes
            .OrderBy(x => RandomUtils.SharedRng.Next())
            .Take(usableColorsCount);

        return ([..selectedPalettes], possiblePalettes.Count());
    }
}

