using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
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
    public List<Pattern> AnimationPossiblePatterns { get; set; } = [..Sfes.GetAll<Pattern>()];
    public PatternChoice AnimationStartingPattern { get; set; } = new PatternChoice.Random();
    public bool AnimationPatternDoesChange { get; set; } = true;
    public double AnimationPatternChangeFrequency { get; set; } = 0.5;

    // Custom palettes are serialized and de-serialized separately. See OptionsSaver.
    [JsonIgnore]
    public List<CustomPaletteSet> CustomPalettes { get; set; } = [];

    public int GetActualSpriteCount(double width, double height)
    {
        var scalingFactor = Interp.Linear(FamilySize, 0.0, 1.0, 0.2, 1.0);

        var count = (width / 64) * (height / 64) * scalingFactor;
        return (int)count;
    }

    public int GetActualMaxColorCount()
    {
        var maxColorCount = FamilyPaletteChoice switch
        {
            PaletteChoice.SingleGroup(PaletteGroup g) => PredefinedPalette.AllForGroup(g).Count(),
            PaletteChoice.AllGroups => Sfes.GetAll<PredefinedPalette>().Count(),
            PaletteChoice.ImFeelingLucky => 30,

            PaletteChoice.UserDefined(var setId, _) => 
                CustomPalettes.FirstOrDefault(s => s.Id == setId)?.Entries.Count ?? 1,

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
        var scale = Interp.Sqrt(IndividualEmotionScale, 0.0, 1.0, 0.0, 2.5);
        return scale;
    }

    public double GetActualPatternChangeFrequencySeconds()
    {
        var seconds = Interp.Linear(AnimationPatternChangeFrequency, 0.0, 1.0, 90.0, 5.0);
        return seconds;
    }

    public double GetActualAnimationSpeedScale()
    {
        var scale = Interp.Square(AnimationSpeed, 0.0, 1.0, 0.05, 0.5);
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


public record CustomPaletteSet(string Id, string Name, List<CustomPaletteEntry> Entries);
public record CustomPaletteEntry(string Name, Palette Palette);
