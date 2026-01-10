using System;
using System.Collections.Generic;
using System.Linq;

using yoksdotnet.common;
using yoksdotnet.drawing;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.logic;

public class ScrOptions
{
    public int ConfigVersion { get; set; } = 1;

    public double FamilyDiversity { get; set; } = 0.2;
    public double FamilySize { get; set; } = 0.5;
    public double FamilyImpostorDensity { get; set; } = 0.1;
    public PaletteChoice FamilyPaletteChoice { get; set; } = new PaletteChoice.SingleGroup(PaletteGroup.XpInspired);

    public double IndividualScale { get; set; } = 0.5;
    public double IndividualEmotionScale { get; set; } = 0.5;
    public bool IndividualTrailsEnabled { get; set; } = false;
    public double IndividualTrailLength { get; set; } = 0.1;

    public double AnimationSpeed { get; set; } = 0.2;
    public List<Pattern> AnimationPossiblePatterns { get; set; } = [..StaticFieldEnumerations.GetAll<Pattern>()];
    public PatternChoice AnimationStartingPattern { get; set; } = new PatternChoice.Random();
    public bool AnimationPatternDoesChange { get; set; } = true;
    public double AnimationPatternChangeFrequency { get; set; } = 0.5;

    public Dictionary<string, Palette> CustomPaletteColors { get; set; } = new() {
        { "Chasnah", new(
            "#6f31dd",
            "#932de3",
            "#3a12a2",
            "#e30efe",
            "#e227ff",
            "#f6f5f4",
            "#e959f5"
        )},
        { "Ellai", new(
            "#97cc72",
            "#caecb1",
            "#338527",
            "#72482d",
            "#f04f2a",
            "#ffffff",
            "#45160e"
        )},
    };

    public int GetActualSpriteCount(double width, double height)
    {
        var scalingFactor = Interpolation.InterpLinear(FamilySize, 0.0, 1.0, 0.2, 1.0);

        var count = (width / 64) * (height / 64) * scalingFactor;
        return (int)count;
    }

    public int GetActualMaxColorCount()
    {
        var maxColorCount = FamilyPaletteChoice switch
        {
            PaletteChoice.SingleGroup(PaletteGroup g) => PredefinedPalette.AllForGroup(g).Count(),
            PaletteChoice.AllGroups => StaticFieldEnumerations.GetAll<PredefinedPalette>().Count(),
            PaletteChoice.ImFeelingLucky => 30,

            _ => throw new NotImplementedException(),
        };

        return maxColorCount;
    }

    public int GetActualColorCount()
    {
        var maxCount = GetActualMaxColorCount();

        var count = Math.Max(2, (int)Math.Round(FamilyDiversity * maxCount));
        return count;
    }
    
    public double GetActualEmotionScale()
    {
        var scale = Interpolation.InterpSqrt(IndividualEmotionScale, 0.0, 1.0, 0.0, 2.5);
        return scale;
    }

    public double GetActualPatternChangeFrequencySeconds()
    {
        var seconds = Interpolation.InterpLinear(AnimationPatternChangeFrequency, 0.0, 1.0, 90.0, 5.0);
        return seconds;
    }

    public double GetActualAnimationSpeedScale()
    {
        var scale = Interpolation.InterpSquare(AnimationSpeed, 0.0, 1.0, 0.05, 0.5);
        return scale;
    }

    // This is used by the runtime debug options window to know whether or not to
    // refresh the entire animation when the option changes.
    // This is necessary for options that affect initial starting conditions such as
    // the amount or color of sprites.
    public static bool PropertyRequiresSceneRefresh(string propertyName)
    {
        List<string> propertiesRequiringRefresh = [
            nameof(FamilyDiversity),
            nameof(FamilySize),
            nameof(FamilyImpostorDensity),
            nameof(FamilyPaletteChoice),
            nameof(IndividualScale),
            nameof(IndividualEmotionScale),
            nameof(AnimationStartingPattern),
        ];

        return propertiesRequiringRefresh.Contains(propertyName);
    }

}
