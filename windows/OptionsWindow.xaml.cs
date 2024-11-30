using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

using yoksdotnet.logic;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.windows;

public partial class OptionsWindow : Window
{
    public List<string> PatternChoices { get; init; } = [..Enum.GetNames<PatternId>()];

    public ScrOptions CurrentOptions { get; private set; }

    private readonly OptionsSaver _optionsSaver = new();
    private readonly OptionsMapper _optionsMapper = new();

    public OptionsWindow()
    {
        InitializeComponent();

        CurrentOptions = _optionsSaver.Load() ?? new();
        DataContext = CurrentOptions;
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        _optionsSaver.Save(CurrentOptions);
    }
}
