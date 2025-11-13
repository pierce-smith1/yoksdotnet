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
    public double IndividualShakiness { get; set; } = 0.5;

    public bool IndividualTrailsEnabled { get; set; } = false;
    public double IndividualTrailLength { get; set; } = 0.1;

    public double AnimationSpeed { get; set; } = 0.5;
    public List<Pattern> AnimationPossiblePatterns { get; set; } = [..StaticFieldEnumeration.GetAll<Pattern>()];
    public PatternChoice AnimationStartingPattern { get; set; } = new PatternChoice.Random();
    public bool AnimationPatternDoesChange { get; set; } = true;
    public double AnimationPatternChangeFrequency { get; set; } = 10.0;
}
