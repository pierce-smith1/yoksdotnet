using System;
using System.Collections.Generic;
using System.Linq;
using yoksdotnet.common;

namespace yoksdotnet.drawing;

public class RandomPaletteGenerator(Random rng)
{
    private readonly RandomSampler _sampler = new(rng);

    public List<Palette> Generate(int amount)
    {
        var parameters = new PaletteGenerationParameters(
            BaseColor: GetRandomSaturatedColor(),
            ColorScrambleIntensity: Interp.Power(rng.NextDouble(), 1.5, 0.0, 1.0, 0.0, 1.0),
            AnomalyChances: new()
            {
                [PaletteAnomaly.Crystalline] = rng.NextDouble() < (1.0 / 50.0) ? 1.0 : 0.1,
                [PaletteAnomaly.DarkEyes] = rng.NextDouble() < (1.0 / 50.0) ? 1.0 : 0.1,
            },
            ColoringStyleWeights: new()
            {
                [PaletteColoringStyle.AllNeutral] = rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 1.0,
                [PaletteColoringStyle.NeutralHorns] = rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 3.0,
                [PaletteColoringStyle.NeutralScales] = rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 1.0,
                [PaletteColoringStyle.AllColored] = rng.NextDouble() < (1.0 / 50.0) ? 100.0 : 0.5,
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

        var nextIsConsonant = rng.NextDouble() < 0.5;
        var phonemeCount = (int)Interp.Sqrt(rng.NextDouble(), 0.0, 1.0, 2.0, 6.0);

        var name = "";
        for (int i = 0; i < phonemeCount; i++)
        {
            name += nextIsConsonant ? _sampler.Sample(consonants) : _sampler.Sample(vowels);
            nextIsConsonant = !nextIsConsonant;
        }

        return $"{name[0..1].ToUpper()}{name[1..]}";
    }

    private RgbColor GetRandomSaturatedColor()
    {
        var h = rng.NextDouble() * 360;
        var s = Interp.Linear(rng.NextDouble(), 0.0, 1.0, 30.0, 50.0);
        var l = Interp.Linear(rng.NextDouble(), 0.0, 1.0, 30.0, 70.0);

        var color = ColorConverter.FromHsl(new(h, s, l));
        return color;
    }

    private Palette GeneratePalette(PaletteGenerationParameters parameters)
    {
        var coloringStyle = _sampler.SampleWeighted(Enum.GetValues<PaletteColoringStyle>(), s => parameters.ColoringStyleWeights[s]);

        var (scalesBase, hornsBase) = coloringStyle switch
        {
            PaletteColoringStyle.AllNeutral => (GenerateNeutralColor(parameters), GenerateNeutralColor(parameters)),
            PaletteColoringStyle.NeutralHorns => (GenerateBaseColor(parameters), GenerateNeutralColor(parameters)),
            PaletteColoringStyle.NeutralScales => (GenerateNeutralColor(parameters), GenerateBaseColor(parameters)),
            PaletteColoringStyle.AllColored => (GenerateBaseColor(parameters), GenerateBaseColor(parameters)),
            _ => throw new NotImplementedException(),
        };

        var palette = new Palette 
        { 
            scales = scalesBase,
            scalesHighlight = LightenColor(scalesBase),
            scalesShadow = DarkenColor(scalesBase),
            horns = hornsBase,
            hornsShadow = DarkenColor(hornsBase),
            eyes = DarkenColor(GenerateBaseColor(parameters)),
            whites = WhitenColor(scalesBase)
        };

        var anomalies = Enum.GetValues<PaletteAnomaly>()
            .Where(a => rng.NextDouble() < parameters.AnomalyChances.GetValueOrDefault(a));

        if (anomalies.Contains(PaletteAnomaly.Crystalline))
        {
            var crystalPalette = new Palette
            {
                scales = palette.scales,
                scalesHighlight = palette.scalesShadow,
                scalesShadow = palette.scalesHighlight,
                horns = palette.hornsShadow,
                hornsShadow = palette.horns,
                eyes = palette.eyes,
                whites = palette.whites,
            };

            palette = crystalPalette;
        }

        if (anomalies.Contains(PaletteAnomaly.DarkEyes))
        {
            palette.whites = new RgbColor(0, 0, 0);
            palette.eyes = LightenColor(GenerateBaseColor(parameters));
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
        var lightness = Interp.Linear(rng.NextDouble(), 0.0, 1.0, 30.0, 70.0);
        var neutralBase = ColorConverter.FromHsl(new(0.0, 0.0, lightness));
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
        var newValue = Interp.Linear(rng.NextDouble(), 0.0, 1.0, min, max);

        return (byte)Math.Clamp(Math.Round(newValue), 0, 255);
    }

    private RgbColor DarkenColor(RgbColor color)
    {
        var (h, s, l) = ColorConverter.ToHsl(color);

        h = ShiftHueTowards(hue: h, target: 240.0, factor: 1.2);
        l /= 2;

        var newColor = ColorConverter.FromHsl(new(h, s, l));
        return newColor;
    }

    private RgbColor LightenColor(RgbColor color)
    {
        var (h, s, l) = ColorConverter.ToHsl(color);

        h = ShiftHueTowards(hue: h, target: 50.0, factor: 1.2);
        l = Interp.Linear(l, 0.0, 100.0, 50.0, 100.0);

        var newColor = ColorConverter.FromHsl(new(h, s, l));
        return newColor;
    }

    private RgbColor WhitenColor(RgbColor color)
    {
        var (h, s, l) = ColorConverter.ToHsl(color);

        l = Interp.Linear(l, 0.0, 100.0, 90.0, 100.0);

        var newColor = ColorConverter.FromHsl(new(h, s, l));
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

