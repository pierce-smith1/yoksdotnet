using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using yoksdotnet.common;
using yoksdotnet.drawing;
using yoksdotnet.logic;
using yoksdotnet.logic.scene.patterns;
using yoksdotnet.windows.common;

namespace yoksdotnet.windows;

public partial class OptionsWindow : Window
{
    public OptionsWindow(ScrOptions? debugOptions = null)
    {
        InitializeComponent();

        if (debugOptions is not null)
        {
            ViewModel.IsDebugWindow = true;
        }

        var options = debugOptions ?? OptionsStorage.Load() ?? new();

        ViewModel.BackingOptions = options;
        InitializeModel(options);
    }

    protected void OnSave(object? _sender, RoutedEventArgs _e)
    {
        OptionsStorage.Save(ViewModel.BackingOptions);
    }

    protected void OnDefaults(object? _sender, RoutedEventArgs _e)
    {
        InitializeModel(new());
    }

    protected void InitializeModel(ScrOptions options)
    {
        ViewModel.FamilyDiversity = options.diversity;
        ViewModel.FamilySize = options.familySize;
        ViewModel.FamilyImpostorDensity = options.impostorDensity;

        ViewModel.IndividualScale = options.spriteScale;
        ViewModel.IndividualEmotionScale = options.emotionScale;
        ViewModel.IndividualTrailsEnabled = options.trailsEnabled;
        ViewModel.IndividualTrailLength = options.trailLength;

        ViewModel.AnimationSpeed = options.animationSpeed;
        ViewModel.AnimationPossiblePatterns = options.possiblePatterns;
        ViewModel.AnimationStartingPattern = new(options.startingPattern);
        ViewModel.AnimationPatternChangeFrequency = options.patternChangeFrequency;
        ViewModel.AnimationPatternDoesChange = options.patternDoesChange;

        ViewModel.CustomPalettes = options.customPalettes;
        ViewModel.FamilyPaletteChoice = PaletteSelection.PaletteSet(options.paletteChoice);
    }

    private void OnCancel(object? _sender, RoutedEventArgs _e)
    {
        Close();
    }

    private void OnEditPaletteSet(object? _sender, RoutedEventArgs _e)
    {
        /*
        if (ViewModel.FamilyPaletteChoice.Set?.CustomSet is not { } setToEdit)
        {
            return;
        }

        var set = ViewModel.CustomPalettes.Single(e => e.Id == setToEdit.Id);

        var paletteCustomizeDialog = new PaletteCustomizer(set);
        if (paletteCustomizeDialog.ShowDialog() is true)
        {
            ViewModel.CustomPalettes = 
            [
                paletteCustomizeDialog.EditedPaletteGroup,
                ..ViewModel.CustomPalettes.Where(e => e.Id != setToEdit.Id)
            ];

            ViewModel.FamilyPaletteChoice = PaletteSelection.PaletteSet(
                PaletteChoice.UserDefined(set.Id, set.Name)
            );

            OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
            OptionsStorage.Save(ViewModel.BackingOptions);
        }
        */
    }

    private void OnDeletePaletteSet(object _sender, RoutedEventArgs _e)
    {
        if (ViewModel.FamilyPaletteChoice.Set?.CustomSet is not { } setToDelete)
        {
            return;
        }

        var group = ViewModel.CustomPalettes.Single(e => e.Id == setToDelete.Id);

        var confirmResult = (group.Entries.Count == 0)
            ? MessageBoxResult.Yes
            : MessageBox.Show
            (
                $"Are you sure you want to delete '{group.Name}' and the {group.Entries.Count} {(group.Entries.Count == 1 ? "yokin" : "yokins")} inside of it?",
                "Confirm deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

        if (confirmResult == MessageBoxResult.Yes)
        {
            ViewModel.FamilyPaletteChoice = PaletteSelection.PaletteSet(new ScrOptions().paletteChoice);

            ViewModel.CustomPalettes = ViewModel.CustomPalettes
                .Where(e => e.Id != setToDelete.Id)
                .ToList();

            OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
        }
    }

    private void OnPaletteSetSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (e.AddedItems is not [var item])
        {
            return;
        }

        if (item is PaletteSelection selection)
        {
            if (selection.Action is PaletteCreateAction.NewSet)
            {
                CreateCustomPaletteSet();
            }
            else if (selection.Action is PaletteCreateAction.ImportPalette)
            {
                ImportCustomPaletteSet();
            }
            else
            {
                return;
            }
        }
    }

    private void CreateCustomPaletteSet()
    {
        /*
        var paletteCustomizeDialog = new PaletteCustomizer(new CustomPaletteSet
        (
            Guid.NewGuid().ToString(),
            "New palette set",
            []
        ));

        if (paletteCustomizeDialog.ShowDialog() is not true)
        {
            ViewModel.FamilyPaletteChoice = PaletteSelection.PaletteSet(
                ViewModel.BackingOptions.paletteChoice
            );

            OptionsStorage.Save(ViewModel.BackingOptions);
            return;
        }

        ViewModel.CustomPalettes = 
        [
            ..ViewModel.CustomPalettes, 
            paletteCustomizeDialog.EditedPaletteGroup
        ];

        var newSetId = paletteCustomizeDialog.EditedPaletteGroup.Id;
        var newSetName = paletteCustomizeDialog.EditedPaletteGroup.Name;

        ViewModel.FamilyPaletteChoice = PaletteSelection.PaletteSet(
            PaletteChoice.UserDefined(newSetId, newSetName)
        );

        OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
        OptionsStorage.Save(ViewModel.BackingOptions);
        */
    }

    private void ImportCustomPaletteSet()
    {
        var paletteImportDialog = new PaletteImportDialog();
        if (paletteImportDialog.ShowDialog() is not true)
        {
            OptionsStorage.Save(ViewModel.BackingOptions);
            return;
        }

        if (paletteImportDialog.ImportedPaletteSet is { } importedSet)
        {
            ViewModel.CustomPalettes =
            [
                ..ViewModel.CustomPalettes,
                importedSet
            ];

            ViewModel.FamilyPaletteChoice = PaletteSelection.PaletteSet(
                PaletteChoice.UserDefined(importedSet.Id, importedSet.Name)
            );

            OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
        }
        else
        {
            ViewModel.FamilyPaletteChoice = PaletteSelection.PaletteSet(
                ViewModel.BackingOptions.paletteChoice
            );
        }
    }

    private void OnCustomizePatternAvailability(object sender, RoutedEventArgs e)
    {
        var customizeDialog = new PatternAvailabilityCustomizer(ViewModel.AnimationPossiblePatterns);
        if (customizeDialog.ShowDialog() is not true)
        {
            return;
        }

        var newPossiblePatterns = customizeDialog.ViewModel.Entries
            .Where(e => e.Enabled)
            .Select(e => e.Pattern)
            .ToList();

        var previousStartingPattern = ViewModel.AnimationStartingPattern;

        var changeNeeded = previousStartingPattern.Choice.Match(
            whenRandom: () => newPossiblePatterns.Count == 1,
            whenSingle: pattern => !newPossiblePatterns.Contains(pattern)
        );

        if (changeNeeded && newPossiblePatterns is [var pattern, ..])
        {
            ViewModel.AnimationStartingPattern = new(PatternChoice.Single(pattern));
        }
        else if (changeNeeded)
        {
            ViewModel.AnimationStartingPattern = new(PatternChoice.Random());
        }

        ViewModel.AnimationPossiblePatterns = newPossiblePatterns;
    }
}

public class OptionsViewModel : NotifiesPropertyChanged
{
    public bool IsDebugWindow { get; set; } = false;
    public bool IsNotDebugWindow => !IsDebugWindow;

    public double FamilyDiversity
    {
        get => BackingOptions.diversity;
        set
        {
            BackingOptions.diversity = value;
            OnPropertyChanged(nameof(FamilyDiversity));
        }
    }

    public double FamilySize
    {
        get => BackingOptions.familySize;
        set
        {
            BackingOptions.familySize = value;
            OnPropertyChanged(nameof(FamilySize));
        }
    }

    public double FamilyImpostorDensity
    {
        get => BackingOptions.impostorDensity;
        set
        {
            BackingOptions.impostorDensity = value;
            OnPropertyChanged(nameof(FamilyImpostorDensity));
        }
    }

    public PaletteSelection FamilyPaletteChoice
    {
        get => PaletteSelection.PaletteSet(BackingOptions.paletteChoice);
        set
        {
            var choice = value.Match(
                whenPaletteSet: choice => choice,
                whenAction: _ => null!
            );

            if (choice is null)
            {
                return;
            }

            BackingOptions.paletteChoice = choice;
            OnPropertyChanged(nameof(FamilyPaletteChoice));
            OnPropertyChanged(nameof(PaletteCustomizeVisibility));
        }
    }

    public double IndividualScale
    {
        get => BackingOptions.spriteScale;
        set
        {
            BackingOptions.spriteScale = value;
            OnPropertyChanged(nameof(IndividualScale));
        }
    }

    public double IndividualEmotionScale
    {
        get => BackingOptions.emotionScale;
        set
        {
            BackingOptions.emotionScale = value;
            OnPropertyChanged(nameof(IndividualEmotionScale));
        }
    }

    public bool IndividualTrailsEnabled
    {
        get => BackingOptions.trailsEnabled;
        set
        {
            BackingOptions.trailsEnabled = value;
            OnPropertyChanged(nameof(IndividualTrailsEnabled));
        }
    }

    public double IndividualTrailLength
    {
        get => BackingOptions.trailLength;
        set
        {
            BackingOptions.trailLength = value;
            OnPropertyChanged(nameof(IndividualTrailLength));
        }
    }

    public double AnimationSpeed
    {
        get => BackingOptions.animationSpeed;
        set
        {
            BackingOptions.animationSpeed = value;
            OnPropertyChanged(nameof(AnimationSpeed));
        }
    }

    public List<Pattern> AnimationPossiblePatterns
    {
        get => BackingOptions.possiblePatterns;
        set
        {
            BackingOptions.possiblePatterns = value;
            OnPropertyChanged(nameof(AnimationPossiblePatterns));
            OnPropertyChanged(nameof(PatternChoices));
        }
    }

    public PatternChoiceEntry AnimationStartingPattern
    {
        get => PatternChoices.SingleOrDefault(e => e.Choice == BackingOptions.startingPattern)!;
        set
        {
            if (value is null)
            {
                return;
            }

            BackingOptions.startingPattern = value.Choice;
            OnPropertyChanged(nameof(AnimationStartingPattern));
        }
    }

    public bool AnimationPatternDoesChange
    {
        get => BackingOptions.patternDoesChange;
        set
        {
            BackingOptions.patternDoesChange = value;
            OnPropertyChanged(nameof(AnimationPatternDoesChange));
        }
    }

    public double AnimationPatternChangeFrequency
    {
        get => BackingOptions.patternChangeFrequency;
        set
        {
            BackingOptions.patternChangeFrequency = value;
            OnPropertyChanged(nameof(AnimationPatternChangeFrequency));
        }
    }

    public List<CustomPaletteSet> CustomPalettes
    {
        get => BackingOptions.customPalettes;
        set
        {
            BackingOptions.customPalettes = value;
            OnPropertyChanged(nameof(CustomPalettes));
            OnPropertyChanged(nameof(PaletteSelections));
        }
    }

    public List<PatternChoiceEntry> PatternChoices
    {
        get
        {
            var choices = AnimationPossiblePatterns
                .Select(p => new PatternChoiceEntry(PatternChoice.Single(p)))
                .ToList();

            if (AnimationPossiblePatterns.Count > 1)
            {
                choices.Add(new(PatternChoice.Random()));
            }

            return choices;
        }
    }

    public Visibility PaletteCustomizeVisibility => FamilyPaletteChoice.Match(
        whenPaletteSet: choice => choice.CustomSet is not null
            ? Visibility.Visible
            : Visibility.Collapsed,
        whenAction: _ => Visibility.Collapsed
    );

    public List<PaletteSelection> PaletteSelections => [
        ..SfEnums.GetAll<PaletteGroup>()
            .Select(g => PaletteSelection.PaletteSet(PaletteChoice.SingleGroup(g))),

        PaletteSelection.PaletteSet(PaletteChoice.All()),
        PaletteSelection.PaletteSet(PaletteChoice.RandomlyGenerated()),

        ..CustomPalettes
            .Select(e => PaletteSelection.PaletteSet(PaletteChoice.UserDefined(e.Id, e.Name))),

        PaletteSelection.CreateAction(PaletteCreateAction.NewSet),
        PaletteSelection.CreateAction(PaletteCreateAction.ImportPalette),
    ];

    public ScrOptions BackingOptions { get; set; } = new();
}

public enum PaletteCreateAction
{
    NewSet,
    ImportPalette
}

public record PaletteSelection
{
    public PaletteChoice? Set { get; init; } = null;
    public PaletteCreateAction? Action { get; init; } = null;

    public static PaletteSelection PaletteSet(PaletteChoice choice) => new()
    {
        Set = choice,
    };

    public static PaletteSelection CreateAction(PaletteCreateAction action) => new()
    {
        Action = action,
    };

    public T Match<T>(Func<PaletteChoice, T> whenPaletteSet, Func<PaletteCreateAction, T> whenAction)
    {
        if (Set is not null) return whenPaletteSet(Set);
        if (Action is not null) return whenAction(Action.Value);

        throw new NotImplementedException();
    }

    public string Name => Match(
        whenPaletteSet: choice => choice!.Match(
            whenSingleGroup: group => group.Name,
            whenAll: () => "Everything",
            whenUserDefined: entry => entry.Name,
            whenGenerated: () => "I'm Feeling Llucky"
        ),

        whenAction: action => action switch
        {
            PaletteCreateAction.NewSet => "Create new palette set...",
            PaletteCreateAction.ImportPalette => "Import palette set...",
            _ => throw new NotImplementedException(),
        }
    );

    public bool IsCustom => Set?.CustomSet is not null;
    public bool IsSpecial => Set is null;
    public FontStyle Style => IsSpecial ? FontStyles.Italic : FontStyles.Normal;
    public string? TextPrefix => IsCustom 
        ? "★" 
        : IsSpecial
        ? "+"
        : null;

    public override string ToString() => Name;
}

public record PatternChoiceEntry(PatternChoice Choice)
{
    public string Name => Choice.Match(
        whenRandom: () => "Random",
        whenSingle: pattern => pattern.Name
    );

    public override string ToString() => Name;
}
