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

    private readonly OptionsSaver _optionsSaver = new();
    private readonly OptionsMapper _optionsMapper = new();

    public OptionsWindow()
    {
        InitializeComponent();

        Resources["currentOptions"] = _optionsSaver.Load() ?? new();
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        var options = Resources["currentOptions"];

        Debug.Print("Saving!!!");
    }
}
