using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using yoksdotnet.common;
using yoksdotnet.drawing;

namespace yoksdotnet.windows;

public partial class AddPaletteDialog : Window
{
    public AddPaletteDialog()
    {
        InitializeComponent();
    }

    private void OnAdd(object _sender, RoutedEventArgs _e)
    {
        DialogResult = true;
    }
}

public class AddPaletteDialogViewModel : INotifyPropertyChanged
{
    public Dictionary<string, Palette> PredefinedPalettes { get; init; } = StaticFieldEnumerations.GetAll<PredefinedPalette>()
        .Select(p => (p.Name, p as Palette))
        .ToDictionary();

    private KeyValuePair<string, Palette> _selectedPalette;
    public KeyValuePair<string, Palette> SelectedPalette
    {
        get => _selectedPalette;
        set
        {
            _selectedPalette = value;
            OnPropertyChanged(nameof(SelectedPalette));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
