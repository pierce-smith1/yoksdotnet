using System;
using System.Collections.Generic;
using System.Windows;

using yoksdotnet.logic.scene.patterns;

namespace yoksdotnet.windows;

public partial class OptionsWindow : Window
{
    public static readonly List<string> PatternChoices = [..Enum.GetNames<PatternId>()];

    public OptionsWindow()
    {
        InitializeComponent();

        PatternSelector.ItemsSource = PatternChoices;
    }
}
