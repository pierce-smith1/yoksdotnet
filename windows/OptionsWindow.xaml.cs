using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using yoksdotnet.common;
using yoksdotnet.drawing;
using yoksdotnet.logic;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.windows;

// Please keep in mind while reading this code that XAML is a momument to human stupidity,
// and any attempt to do anything remotely useful will necessarily result in a sprawling
// wasteland of meaningless and error-prone boilerplate.
//
// If it looks stupid, it is, and you will just have to live with that.
// Press CTRL+C, then CTRL+V, shed a tear for the time you will soon waste debugging a
// subtle bug as the result of your copy-pasting, and then move on.

public partial class OptionsWindow : Window
{
    private readonly OptionsSaver _optionsSaver = new();

    public OptionsWindow(ScrOptions? debugOptions = null)
    {
        InitializeComponent();

        if (debugOptions is not null)
        {
            ViewModel.IsDebugWindow = true;
        }

        var options = debugOptions ?? _optionsSaver.Load() ?? new();

        ViewModel.BackingOptions = options;
        InitializeModel(options);
    }

    protected void OnSave(object? _sender, RoutedEventArgs _e)
    {
        _optionsSaver.Save(ViewModel.BackingOptions);
    }

    protected void OnDefaults(object? _sender, RoutedEventArgs _e)
    {
        InitializeModel(new());
    }

    protected void InitializeModel(ScrOptions options)
    {
        ViewModel.FamilyDiversity = options.FamilyDiversity;
        ViewModel.FamilySize = options.FamilySize;
        ViewModel.FamilyImpostorDensity = options.FamilyImpostorDensity;
        ViewModel.FamilyPaletteChoice = options.FamilyPaletteChoice;

        ViewModel.IndividualScale = options.IndividualScale;
        ViewModel.IndividualEmotionScale = options.IndividualEmotionScale;
        ViewModel.IndividualTrailsEnabled = options.IndividualTrailsEnabled;
        ViewModel.IndividualTrailLength = options.IndividualTrailLength;

        ViewModel.AnimationSpeed = options.AnimationSpeed;
        ViewModel.AnimationPossiblePatterns = options.AnimationPossiblePatterns;
        ViewModel.AnimationStartingPattern = options.AnimationStartingPattern;
        ViewModel.AnimationPatternChangeFrequency = options.AnimationPatternChangeFrequency;
        ViewModel.AnimationPatternDoesChange = options.AnimationPatternDoesChange;
    }

    private void OnCancel(object? _sender, RoutedEventArgs _e)
    {
        Close();
    }

    private void OnCustomizePalettes(object? _sender, RoutedEventArgs _e)
    {
        var paletteCustomizeDialog = new PaletteCustomizer(ViewModel.BackingOptions.CustomPalettes);
        if (paletteCustomizeDialog.ShowDialog() is true)
        {
            ViewModel.BackingOptions.CustomPalettes = paletteCustomizeDialog.GetPaletteState();
        }
    }
}

public class OptionsViewModel : INotifyPropertyChanged
{
    public bool IsDebugWindow { get; set; } = false;
    public bool IsNotDebugWindow => !IsDebugWindow;

    public double FamilyDiversity 
    {
        get => BackingOptions.FamilyDiversity;
        set 
        {
            BackingOptions.FamilyDiversity = value;
            OnPropertyChanged(nameof(FamilyDiversity));
        }
    }

    public double FamilySize
    {
        get => BackingOptions.FamilySize;
        set 
        {
            BackingOptions.FamilySize = value;
            OnPropertyChanged(nameof(FamilySize));
        }
    }

    public double FamilyImpostorDensity
    {
        get => BackingOptions.FamilyImpostorDensity;
        set
        {
            BackingOptions.FamilyImpostorDensity = value;
            OnPropertyChanged(nameof(FamilyImpostorDensity));
        }
    }

    public PaletteChoice FamilyPaletteChoice 
    {
        get => BackingOptions.FamilyPaletteChoice;
        set 
        {
            BackingOptions.FamilyPaletteChoice = value;
            OnPropertyChanged(nameof(FamilyPaletteChoice));
            OnPropertyChanged(nameof(PaletteCustomizeVisibility));
        }
    }

    public double IndividualScale
    {
        get => BackingOptions.IndividualScale;
        set 
        {
            BackingOptions.IndividualScale = value;
            OnPropertyChanged(nameof(IndividualScale));
        }
    }

    public double IndividualEmotionScale
    {
        get => BackingOptions.IndividualEmotionScale;
        set 
        {
            BackingOptions.IndividualEmotionScale = value;
            OnPropertyChanged(nameof(IndividualEmotionScale));
        }
    }

    public bool IndividualTrailsEnabled
    {
        get => BackingOptions.IndividualTrailsEnabled;
        set 
        {
            BackingOptions.IndividualTrailsEnabled = value;
            OnPropertyChanged(nameof(IndividualTrailsEnabled));
        }
    }

    public double IndividualTrailLength
    {
        get => BackingOptions.IndividualTrailLength;
        set 
        {
            BackingOptions.IndividualTrailLength = value;
            OnPropertyChanged(nameof(IndividualTrailLength));
        }
    }

    public double AnimationSpeed
    {
        get => BackingOptions.AnimationSpeed;
        set 
        {
            BackingOptions.AnimationSpeed = value;
            OnPropertyChanged(nameof(AnimationSpeed));
        }
    }

    public List<Pattern> AnimationPossiblePatterns
    {
        get => BackingOptions.AnimationPossiblePatterns;
        set 
        {
            BackingOptions.AnimationPossiblePatterns = value;
            OnPropertyChanged(nameof(AnimationPossiblePatterns));
            OnPropertyChanged(nameof(PatternChoices));
        }
    }

    public PatternChoice AnimationStartingPattern
    {
        get => BackingOptions.AnimationStartingPattern;
        set 
        {
            BackingOptions.AnimationStartingPattern = value;

            OnPropertyChanged(nameof(AnimationStartingPattern));
        }
    }

    public bool AnimationPatternDoesChange
    {
        get => BackingOptions.AnimationPatternDoesChange;
        set 
        {
            BackingOptions.AnimationPatternDoesChange = value;
            OnPropertyChanged(nameof(AnimationPatternDoesChange));
        }
    }

    public double AnimationPatternChangeFrequency
    {
        get => BackingOptions.AnimationPatternChangeFrequency;
        set 
        {
            BackingOptions.AnimationPatternChangeFrequency = value;
            OnPropertyChanged(nameof(AnimationPatternChangeFrequency));
        }
    }

    public Visibility PaletteCustomizeVisibility => (FamilyPaletteChoice is PaletteChoice.UserDefined)
        ? Visibility.Visible
        : Visibility.Hidden;

    public List<PaletteChoice> PaletteChoices { get; init; } = [
        ..StaticFieldEnumerations.GetAll<PaletteGroup>().Select(g => new PaletteChoice.SingleGroup(g)),
        new PaletteChoice.AllGroups(),
        new PaletteChoice.ImFeelingLucky(),
        new PaletteChoice.UserDefined(),
    ];

    public List<PatternChoice> PatternChoices => [
        ..AnimationPossiblePatterns.Select(p => new PatternChoice.SinglePattern(p)),
        new PatternChoice.Random(),
    ];

    public ScrOptions BackingOptions { get; set; } = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
