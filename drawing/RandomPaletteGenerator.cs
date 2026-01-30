using System;
using System.Collections.Generic;
using System.Linq;
using yoksdotnet.common;

namespace yoksdotnet.drawing;

public class RandomPaletteGenerator(Random _rng)
{
    public List<Palette> Generate(int amount)
    {
        var parameters = new PaletteGenerationParameters(
            BaseColor: GetRandomSaturatedColor(),
            ColorScrambleIntensity: Interp.Power(_rng.NextDouble(), 1.5, 0.0, 1.0, 0.0, 1.0),
            AnomalyChances: new()
            {
                [PaletteAnomaly.Crystalline] = _rng.NextDouble() < (1.0 / 50.0) ? 1.0 : 0.1,
                [PaletteAnomaly.DarkEyes] = _rng.NextDouble() < (1.0 / 50.0) ? 1.0 : 0.1,
            },
            ColoringStyleWeights: new()
            {
                [PaletteColoringStyle.AllNeutral] = _rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 1.0,
                [PaletteColoringStyle.NeutralHorns] = _rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 3.0,
                [PaletteColoringStyle.NeutralScales] = _rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 1.0,
                [PaletteColoringStyle.AllColored] = _rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 0.5,
            }
        );

        var palettes = Enumerable.Range(0, amount)
            .Select(_ => GeneratePalette(parameters))
            .ToList();

        return palettes;
    }

    public string NamePalette(Palette _palette)
    {
        List<string> consonants = [
            "s",
            "w",
            "z",
            "j",
            "ll",
            "x",
            "m",
            "zs",
            "ng",
            "v",
            "l",
            "b",
            "g",
            "r",
            "tch",
            "t",
            "k",
            "d",
            "n"
        ];

        List<string> vowels = [
            "a",
            "ae",
            "e",
            "eh",
            "u",
            "o",
            "i",
            "ii",
        ];

        var nextIsConsonant = _rng.NextDouble() < 0.5;
        var phonemeCount = (int)Interp.Sqrt(_rng.NextDouble(), 0.0, 1.0, 2.0, 6.0);

        var name = "";
        for (int i = 0; i < phonemeCount; i++)
        {
            name += nextIsConsonant ? consonants.Sample(_rng) : vowels.Sample(_rng);
            nextIsConsonant = !nextIsConsonant;
        }

        return $"{name[0..1].ToUpper()}{name[1..]}";
    }

    private RgbColor GetRandomSaturatedColor()
    {
        var h = _rng.NextDouble() * 360;
        var s = Interp.Linear(_rng.NextDouble(), 0.0, 1.0, 30.0, 50.0);
        var l = Interp.Linear(_rng.NextDouble(), 0.0, 1.0, 30.0, 70.0);

        var color = RgbColor.FromHsl(h, s, l);
        return color;
    }

    private Palette GeneratePalette(PaletteGenerationParameters parameters)
    {
        var coloringStyle = Enum.GetValues<PaletteColoringStyle>()
            .SampleWeighted(s => parameters.ColoringStyleWeights[s], _rng);

        var (scalesBase, hornsBase) = coloringStyle switch
        {
            PaletteColoringStyle.AllNeutral => (GenerateNeutralColor(parameters), GenerateNeutralColor(parameters)),
            PaletteColoringStyle.NeutralHorns => (GenerateBaseColor(parameters), GenerateNeutralColor(parameters)),
            PaletteColoringStyle.NeutralScales => (GenerateNeutralColor(parameters), GenerateBaseColor(parameters)),
            PaletteColoringStyle.AllColored => (GenerateBaseColor(parameters), GenerateBaseColor(parameters)),
            _ => throw new NotImplementedException(),
        };

        var palette = new Palette(
            s: scalesBase,
            sh: LightenColor(scalesBase),
            ss: DarkenColor(scalesBase),
            h: hornsBase,
            hs: DarkenColor(hornsBase),
            e: DarkenColor(GenerateBaseColor(parameters)),
            w: WhitenColor(scalesBase)
        );

        var anomalies = Enum.GetValues<PaletteAnomaly>()
            .Where(a => _rng.NextDouble() < parameters.AnomalyChances.GetValueOrDefault(a));

        if (anomalies.Contains(PaletteAnomaly.Crystalline))
        {
            var crystalPalette = new Palette(palette)
            {
                ScalesHighlight = palette.ScalesShadow,
                ScalesShadow = palette.ScalesHighlight,

                Horns = palette.HornsShadow,
                HornsShadow = palette.Horns
            };

            palette = crystalPalette;
        }

        if (anomalies.Contains(PaletteAnomaly.DarkEyes))
        {
            palette.Whites = new RgbColor(0, 0, 0);
            palette.Eyes = LightenColor(GenerateBaseColor(parameters));
        }

        return palette;
    }

    private RgbColor GenerateBaseColor(PaletteGenerationParameters parameters)
    {
        var baseColor = ScrambleColor(parameters.BaseColor, parameters.ColorScrambleIntensity);
        return baseColor;
    }

    private RgbColor GenerateNeutralColor(PaletteGenerationParameters parameters)
    {
        var neutralBase = RgbColor.FromHsl(0.0, 0.0, Interp.Linear(_rng.NextDouble(), 0.0, 1.0, 30.0, 70.0));
        var neutralColor = ScrambleColor(neutralBase, 0.2);
        return neutralColor;
    }

    private RgbColor ScrambleColor(RgbColor color, double intensity)
    {
        var newColor = new RgbColor
        (
            ScrambleColorChannel(color.R, intensity),
            ScrambleColorChannel(color.G, intensity),
            ScrambleColorChannel(color.B, intensity)
        );

        return newColor;
    }

    private byte ScrambleColorChannel(byte value, double intensity)
    {
        var scrambleRange = intensity * 255;

        var min = Math.Max(value - scrambleRange + Math.Max(value + scrambleRange - 255, 0), 0);
        var max = Math.Min(value + scrambleRange - Math.Min(value - scrambleRange + 255, 0), 255);
        var newValue = Interp.Linear(_rng.NextDouble(), 0.0, 1.0, min, max);

        return (byte)Math.Clamp(Math.Round(newValue), 0, 255);
    }

    private RgbColor DarkenColor(RgbColor color)
    {
        var (h, s, l) = color.ToHsl();

        h = ShiftHueTowards(hue: h, target: 240.0, factor: 1.2);
        l /= 2;

        var newColor = RgbColor.FromHsl(h, s, l);
        return newColor;
    }

    private RgbColor LightenColor(RgbColor color)
    {
        var (h, s, l) = color.ToHsl();

        h = ShiftHueTowards(hue: h, target: 50.0, factor: 1.2);
        l = Interp.Linear(l, 0.0, 100.0, 50.0, 100.0);

        var newColor = RgbColor.FromHsl(h, s, l);
        return newColor;
    }

    private RgbColor WhitenColor(RgbColor color)
    {
        var (h, s, l) = color.ToHsl();

        l = Interp.Linear(l, 0.0, 100.0, 90.0, 100.0);

        var newColor = RgbColor.FromHsl(h, s, l);
        return newColor;
    }

    private double ShiftHueTowards(double hue, double target, double factor)
    {
        hue -= target;
        hue = Interp.Linear(hue, -target, 360 - target, -target / factor, (360 - target) / factor);
        hue += target;

        return hue;
    }
}

public record PaletteGenerationParameters
(
    RgbColor BaseColor,
    double ColorScrambleIntensity,
    Dictionary<PaletteAnomaly, double> AnomalyChances,
    Dictionary<PaletteColoringStyle, double> ColoringStyleWeights
);

public enum PaletteColoringStyle
{
    NeutralHorns,
    NeutralScales,
    AllNeutral,
    AllColored,
}

public enum PaletteAnomaly
{
    Crystalline,
    DarkEyes,
}

