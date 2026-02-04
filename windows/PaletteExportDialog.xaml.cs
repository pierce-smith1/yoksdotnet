using System.ComponentModel;
using System.Windows;
using yoksdotnet.logic;

namespace yoksdotnet.windows;

public partial class PaletteExportDialog : Window
{
    public PaletteExportDialog(CustomPaletteSet group)
    {
        InitializeComponent();

        ViewModel.ExportCode = PaletteExporting.Export(group);
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class PaletteExportViewModel : INotifyPropertyChanged
{
    private string? _exportCode;
    public string? ExportCode
    {
        get => _exportCode;
        set
        {
            _exportCode = value;
            OnPropertyChanged(nameof(ExportCode));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
