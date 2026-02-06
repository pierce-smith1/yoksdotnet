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
        ViewModel.FamilyPaletteChoice = new(options.paletteChoice);
    }

    private void OnCancel(object? _sender, RoutedEventArgs _e)
    {
        Close();
    }

    private void OnEditPaletteSet(object? _sender, RoutedEventArgs _e)
    {
        if (ViewModel.FamilyPaletteChoice.Choice is not UserDefinedPalettes groupChoice)
        {
            return;
        }

        var set = ViewModel.CustomPalettes.Single(e => e.Id == groupChoice.SetId);

        var paletteCustomizeDialog = new PaletteCustomizer(set);
        if (paletteCustomizeDialog.ShowDialog() is true)
        {
            ViewModel.CustomPalettes = 
            [
                paletteCustomizeDialog.EditedPaletteGroup,
                ..ViewModel.CustomPalettes.Where(e => e.Id != groupChoice.SetId)
            ];

            ViewModel.FamilyPaletteChoice = ViewModel.PaletteChoices
                .First(c => c.Choice is UserDefinedPalettes ud && ud.SetId == set.Id);

            OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
            OptionsStorage.Save(ViewModel.BackingOptions);
        }
    }

    private void OnDeletePaletteSet(object _sender, RoutedEventArgs _e)
    {
        if (ViewModel.FamilyPaletteChoice.Choice is not UserDefinedPalettes groupChoice)
        {
            return;
        }

        var group = ViewModel.CustomPalettes.Single(e => e.Id == groupChoice.SetId);

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
            ViewModel.FamilyPaletteChoice = new(new ScrOptions().paletteChoice);

            ViewModel.CustomPalettes = ViewModel.CustomPalettes
                .Where(e => e.Id != groupChoice.SetId)
                .ToList();

            OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
        }
    }

    private void OnPaletteSetSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (e.AddedItems is not { Count: 1 })
        {
            return;
        }

        if (e.AddedItems[0] is PaletteChoiceEntry entry)
        {
            if (entry.SpecialEntry is NewSetEntry)
            {
                CreateCustomPaletteSet();
            }
            else if (entry.SpecialEntry is ImportPaletteEntry)
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
        var paletteCustomizeDialog = new PaletteCustomizer(new CustomPaletteSet
        (
            Guid.NewGuid().ToString(),
            "New palette set",
            []
        ));

        if (paletteCustomizeDialog.ShowDialog() is true)
        {
            ViewModel.CustomPalettes = 
            [
                ..ViewModel.CustomPalettes, 
                paletteCustomizeDialog.EditedPaletteGroup
            ];

            var newSetId = paletteCustomizeDialog.EditedPaletteGroup.Id;

            ViewModel.FamilyPaletteChoice = ViewModel.PaletteChoices
                .First(c => c.Choice is UserDefinedPalettes ud && ud.SetId == newSetId);

            OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
        }
        else
        {
            ViewModel.FamilyPaletteChoice = new(ViewModel.BackingOptions.paletteChoice);
        }

        OptionsStorage.Save(ViewModel.BackingOptions);
    }

    private void ImportCustomPaletteSet()
    {
        var paletteImportDialog = new PaletteImportDialog();
        if (paletteImportDialog.ShowDialog() is true && paletteImportDialog.ImportedPaletteSet is { } importedSet)
        {
            ViewModel.CustomPalettes =
            [
                ..ViewModel.CustomPalettes,
                importedSet
            ];

            ViewModel.FamilyPaletteChoice = ViewModel.PaletteChoices
                .First(c => c.Choice is UserDefinedPalettes ud && ud.SetId == importedSet.Id);

            OptionsStorage.SaveCustomPalettes(ViewModel.CustomPalettes);
        }
        else
        {
            ViewModel.FamilyPaletteChoice = new(ViewModel.BackingOptions.paletteChoice);
        }

        OptionsStorage.Save(ViewModel.BackingOptions);
    }

    private void OnCustomizePatternAvailability(object sender, RoutedEventArgs e)
    {
        var customizeDialog = new PatternAvailabilityCustomizer(ViewModel.AnimationPossiblePatterns);
        if (customizeDialog.ShowDialog() is true)
        {
            var newPossiblePatterns = customizeDialog.ViewModel.Entries
                .Where(e => e.Enabled)
                .Select(e => e.Pattern)
                .ToList();

            var previousStartingPattern = ViewModel.AnimationStartingPattern;

            ViewModel.AnimationPossiblePatterns = newPossiblePatterns;

            if (previousStartingPattern.Choice is RandomPatternChoice && newPossiblePatterns.Count == 1)
            {
                ViewModel.AnimationStartingPattern = ViewModel.PatternChoices.First();
            }

            if (previousStartingPattern.Choice is SinglePatternChoice(var pattern) && !ViewModel.AnimationPossiblePatterns.Any(e => e == pattern))
            {
                if (ViewModel.AnimationPossiblePatterns.Count == 1)
                {
                    ViewModel.AnimationStartingPattern = new(new SinglePatternChoice(ViewModel.AnimationPossiblePatterns.First()));
                }
                else
                {
                    ViewModel.AnimationStartingPattern = new(new RandomPatternChoice());
                }
            }
        }
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

    public PaletteChoiceEntry FamilyPaletteChoice
    {
        get => new(BackingOptions.paletteChoice);
        set
        {
            if (value?.Choice is null)
            {
                return;
            }

            BackingOptions.paletteChoice = value.Choice;
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
            OnPropertyChanged(nameof(PaletteChoices));
        }
    }

    public List<PatternChoiceEntry> PatternChoices
    {
        get
        {
            var choices = AnimationPossiblePatterns
                .Select(p => new PatternChoiceEntry(new SinglePatternChoice(p)))
                .ToList();

            if (AnimationPossiblePatterns.Count > 1)
            {
                choices.Add(new(new RandomPatternChoice()));
            }

            return choices;
        }
    }

    public Visibility PaletteCustomizeVisibility => (FamilyPaletteChoice.Choice is UserDefinedPalettes)
        ? Visibility.Visible
        : Visibility.Hidden;

    public List<PaletteChoiceEntry> PaletteChoices => [
        ..SfEnums.GetAll<PaletteGroup>()
            .Select(g => new PaletteChoiceEntry(new SingleGroupPalettes(g))),

        new(new AllPalettes()),
        new(new GeneratedPalettes()),

        ..CustomPalettes
            .Select(e => new PaletteChoiceEntry(new UserDefinedPalettes(e.Id, e.Name))),

        new(new NewSetEntry()),
        new(new ImportPaletteEntry()),
    ];

    public ScrOptions BackingOptions { get; set; } = new();
}

public record SpecialPaletteEntry : Union<NewSetEntry, ImportPaletteEntry>;
public record NewSetEntry : SpecialPaletteEntry;
public record ImportPaletteEntry : SpecialPaletteEntry;

public record PaletteChoiceEntry
{

    public PaletteChoice? Choice { get; init; }
    public SpecialPaletteEntry? SpecialEntry { get; init; }

    public PaletteChoiceEntry(PaletteChoice choice)
    {
        Choice = choice;
    }

    public PaletteChoiceEntry(SpecialPaletteEntry entry)
    {
        SpecialEntry = entry;
    }

    public string Name => ToString();
    public bool IsCustom => Choice is UserDefinedPalettes;
    public bool IsSpecial => Choice is null;
    public FontStyle Style => IsSpecial ? FontStyles.Italic : FontStyles.Normal;
    public string? TextPrefix => IsCustom 
        ? "★" 
        : IsSpecial
        ? "+"
        : null;

    public override string ToString() => Choice?.Match(
        singleGroup => singleGroup.Group.Name,
        _all => "Everything",
        userDefined => userDefined.Name,
        _generated => "I'm Feeling Llucky"
    ) ?? SpecialEntry?.Match(
        _newSet => "New custom palette set...",
        _importPalette => "Import palette set..."
    ) ?? throw new NotSupportedException();
}

public record PatternChoiceEntry(PatternChoice Choice)
{
    public string Name => Choice.Match(
        _random => "Random",
        single => single.Pattern.Name
    );

    public override string ToString() => Name;
}
