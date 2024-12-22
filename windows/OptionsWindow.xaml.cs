using System;
using System.Collections.Generic;
using System.Windows;

using yoksdotnet.logic;
using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.windows;

public partial class OptionsWindow : Window
{
    public List<string> PatternChoices { get; init; } = [..Enum.GetNames<PatternId>()];

    public ScrOptions CurrentOptions { get; private set; }

    private readonly OptionsSaver _optionsSaver = new();

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

public class LabeledSliderElement : FrameworkElement
{
    public required string LeftLabel { get; init; }
    public required string RightLabel { get; init; }

    // I tried so hard
    // and got so far
    // but in the end
    // it doesn't even matter
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
    (
        "Value",
        typeof(double),
        typeof(LabeledSliderElement)
    );

    public double Value
    {
        get => (double) GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
