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

        for (var i = 0; i < Options.GetSpriteCount(); i++)
        {
            var newSprite = new Yokin
            {
                Id = _runningId++,
                Brand = rng.NextDouble(),
                Home = new(rng.NextDouble() * spreadX, rng.NextDouble() * spreadY),
                Offset = new(0, 0),
                Scale = Options.IndividualScale,
                Width = 128,
                Height = 128,
                AngleRadians = 0.0,
                Paint = rng.SampleExponential(selectedPalettes, 1 - (double)selectedPalettes.Count() / totalPossibleCount).Paint,
                Emotion = new(),
            };

            sprites.Add(newSprite);
        }

        return sprites;
    }

    private (List<Palette> Palettes, int TotalPossibleCount) SelectPalettes()
    {
        var possiblePalettes = Options.FamilyPaletteChoice switch
        {
            PaletteChoice.SingleGroup choice => StaticFieldEnumeration.GetAll<Palette>().Where(pair => pair.Group == choice.Group),

            _ => throw new NotImplementedException(),
        };

        var usableColorsCount = Math.Min(Options.GetColorCount(), possiblePalettes.Count());

        var selectedPalettes = possiblePalettes
            .OrderBy(x => RandomUtils.SharedRng.Next())
            .Take(usableColorsCount);

        return ([..selectedPalettes], possiblePalettes.Count());
    }
}

