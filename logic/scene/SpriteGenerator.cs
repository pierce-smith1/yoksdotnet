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

        List<Sprite> entities = new(Options.SpriteCount);

        var (selectedPalettes, totalPossibleCount) = SelectPalettes();

        for (var i = 0; i < Options.SpriteCount; i++)
        {
            var newEntity = new Yokin
            {
                Id = _runningId++,
                Brand = rng.NextDouble(),
                Home = new(rng.NextDouble() * spreadX, rng.NextDouble() * spreadY),
                Offset = new(0, 0),
                Scale = Options.SpriteScale,
                Width = 128,
                Height = 128,
                AngleRadians = 0.0,
                Paint = Palettes.Paints[rng.SampleExponential(selectedPalettes, 1 - (double)selectedPalettes.Count() / totalPossibleCount)],
                Emotion = new(),
            };

            entities.Add(newEntity);
        }

        return entities;
    }

    private (List<PaletteId> Palettes, int TotalPossibleCount) SelectPalettes()
    {
        var possiblePalettes = Options.PaletteChoice switch
        {
            PaletteChoice.SingleGroup choice => Palettes.Definitions
                .Where(pair => pair.Value.Group == choice.Group)
                .Select(pair => pair.Key),

            _ => throw new NotImplementedException(),
        };

        var usableColorsCount = Math.Min(Options.ColorsCount, possiblePalettes.Count());

        var selectedPalettes = possiblePalettes
            .OrderBy(x => RandomUtils.SharedRng.Next())
            .Take(usableColorsCount);

        return ([..selectedPalettes], possiblePalettes.Count());
    }
}
