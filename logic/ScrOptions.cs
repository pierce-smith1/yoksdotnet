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
    public int ConfigVersion = 1;

    public double FamilyDiversity = 0.2;
    public double FamilySize = 0.5;
    public double FamilyImpostorDensity = 0.1;
    public PaletteChoice FamilyPaletteChoice = new PaletteChoice.SingleGroup(PaletteGroup.XpInspired);

    public double IndividualScale = 0.5;
    public double IndividualEmotionScale = 0.5;
    public bool IndividualTrailsEnabled = false;
    public double IndividualTrailLength = 0.1;

    public double AnimationSpeed = 0.2;
    public List<Pattern> AnimationPossiblePatterns = [.. SfEnums.GetAll<Pattern>()];
    public PatternChoice AnimationStartingPattern = new RandomPatternChoice();
    public bool AnimationPatternDoesChange = true;
    public double AnimationPatternChangeFrequency = 0.5;

    // Custom palettes are serialized and de-serialized separately. See OptionsStore.
    [JsonIgnore]
    public List<CustomPaletteSet> CustomPalettes { get; set; } = [];

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
