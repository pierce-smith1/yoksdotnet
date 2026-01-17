using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using yoksdotnet.logic;

namespace yoksdotnet.windows;

public partial class PaletteImportDialog : Window
{
    private readonly PaletteExporter _exporter = new();

    public CustomPaletteSet? ImportedPaletteSet { get; set; }

    public PaletteImportDialog()
    {
        InitializeComponent();
    }

    public void OnCancel(object _sender, RoutedEventArgs _e)
    {
        DialogResult = false;
    }

    public void OnImport(object _sender, RoutedEventArgs _e)
    {
        var paletteSet = _exporter.Import(ViewModel.ImportCode);

        if (paletteSet is null)
        {
            MessageBox.Show
            (
                "This doesn't appear to be a valid palette code. Make sure it was copied correctly.",
                "Import failed",
                MessageBoxButton.OK,
                MessageBoxImage.Stop
            );

            return;
        }

        ImportedPaletteSet = paletteSet;
        DialogResult = true;
    }
}

public class PaletteImportViewModel
{
    public string ImportCode { get; set; } = "";
}
