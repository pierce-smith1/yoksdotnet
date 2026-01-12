using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        ViewModel.IndividualScale = options.IndividualScale;
        ViewModel.IndividualEmotionScale = options.IndividualEmotionScale;
        ViewModel.IndividualTrailsEnabled = options.IndividualTrailsEnabled;
        ViewModel.IndividualTrailLength = options.IndividualTrailLength;

        ViewModel.AnimationSpeed = options.AnimationSpeed;
        ViewModel.AnimationPossiblePatterns = options.AnimationPossiblePatterns;
        ViewModel.AnimationStartingPattern = options.AnimationStartingPattern;
        ViewModel.AnimationPatternChangeFrequency = options.AnimationPatternChangeFrequency;
        ViewModel.AnimationPatternDoesChange = options.AnimationPatternDoesChange;

        ViewModel.CustomPalettes = options.CustomPalettes;
        ViewModel.FamilyPaletteChoice = new(options.FamilyPaletteChoice);
    }

    private void OnCancel(object? _sender, RoutedEventArgs _e)
    {
        Close();
    }

    private void OnEditPaletteGroup(object? _sender, RoutedEventArgs _e)
    {
        if (ViewModel.FamilyPaletteChoice.Choice is not PaletteChoice.UserDefined groupChoice)
        {
            return;
        }

        var group = ViewModel.CustomPalettes.Single(e => e.Name == groupChoice.GroupName);

        var paletteCustomizeDialog = new PaletteCustomizer(group.Name, group.Entries);
        if (paletteCustomizeDialog.ShowDialog() is true)
        {
            var newGroupName = paletteCustomizeDialog.EditedPaletteGroup.Name;

            ViewModel.FamilyPaletteChoice = new(new PaletteChoice.UserDefined(newGroupName));

            ViewModel.CustomPalettes = 
            [
                paletteCustomizeDialog.EditedPaletteGroup,
                ..ViewModel.CustomPalettes.Where(e => e.Name != groupChoice.GroupName)
            ];

            _optionsSaver.SaveCustomPalettes(ViewModel.CustomPalettes);
        }
    }

    private void OnDeletePaletteGroup(object _sender, RoutedEventArgs _e)
    {
        if (ViewModel.FamilyPaletteChoice.Choice is not PaletteChoice.UserDefined groupChoice)
        {
            return;
        }

        var group = ViewModel.CustomPalettes.Single(e => e.Name == groupChoice.GroupName);

        var confirmResult = (group.Entries.Count == 0)
            ? MessageBoxResult.Yes
            : MessageBox.Show
            (
                $"Are you sure you want to delete '{groupChoice.GroupName}' and the {group.Entries.Count} {(group.Entries.Count == 1 ? "yokin" : "yokins")} inside of it?",
                "Confirm deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

        if (confirmResult == MessageBoxResult.Yes)
        {
            ViewModel.FamilyPaletteChoice = new(new ScrOptions().FamilyPaletteChoice);

            ViewModel.CustomPalettes = ViewModel.CustomPalettes
                .Where(e => e.Name != groupChoice.GroupName)
                .ToList();

            _optionsSaver.Save(ViewModel.BackingOptions);
        }
    }

    private void OnPaletteSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (e.AddedItems is not { Count: 1 })
        {
            return;
        }

        if (e.AddedItems[0] is PaletteChoiceEntry entry && entry.Choice is not null)
        {
            return;
        }

        CreateCustomPaletteSet();
    }

    private void CreateCustomPaletteSet()
    {
        var paletteCustomizeDialog = new PaletteCustomizer("New palette set", []);
        if (paletteCustomizeDialog.ShowDialog() is true)
        {
            ViewModel.CustomPalettes = 
            [
                ..ViewModel.CustomPalettes, 
                paletteCustomizeDialog.EditedPaletteGroup
            ];

            ViewModel.FamilyPaletteChoice = new(new PaletteChoice.UserDefined(paletteCustomizeDialog.EditedPaletteGroup.Name));

            _optionsSaver.SaveCustomPalettes(ViewModel.CustomPalettes);
        }
        else
        {
            ViewModel.FamilyPaletteChoice = new(ViewModel.BackingOptions.FamilyPaletteChoice);
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

    public PaletteChoiceEntry FamilyPaletteChoice 
    {
        get => new(BackingOptions.FamilyPaletteChoice);
        set 
        {
            if (value.Choice is null)
            {
                return;
            }

            BackingOptions.FamilyPaletteChoice = value.Choice;
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

    public List<CustomPaletteGroup> CustomPalettes
    {
        get => BackingOptions.CustomPalettes;
        set
        {
            BackingOptions.CustomPalettes = value;
            OnPropertyChanged(nameof(CustomPalettes));
            OnPropertyChanged(nameof(PaletteChoices));
        }
    }

    public Visibility PaletteCustomizeVisibility => (FamilyPaletteChoice.Choice is PaletteChoice.UserDefined)
        ? Visibility.Visible
        : Visibility.Hidden;

    public List<PaletteChoiceEntry> PaletteChoices => [
        ..StaticFieldEnumerations.GetAll<PaletteGroup>()
            .Select(g => new PaletteChoiceEntry(new PaletteChoice.SingleGroup(g))),
        new(new PaletteChoice.AllGroups()),
        new(new PaletteChoice.ImFeelingLucky()),
        ..CustomPalettes
            .Select(e => new PaletteChoiceEntry(new PaletteChoice.UserDefined(e.Name))),
        new(null),
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

public record PaletteChoiceEntry(PaletteChoice? Choice)
{
    public override string ToString() => Choice switch
    {
        PaletteChoice.SingleGroup(var group) => group.Name,
        PaletteChoice.AllGroups => "Everything",
        PaletteChoice.UserDefined(var name) => $"{name} (custom)",
        PaletteChoice.ImFeelingLucky => "I'm Feeling Llucky",
        null => "Create new palette set...",

        _ => throw new InvalidOperationException(),
    };
}
