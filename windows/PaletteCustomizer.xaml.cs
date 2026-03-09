using Microsoft.Win32;
using SkiaSharp.Views.Desktop;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using yoksdotnet.common;
using yoksdotnet.data;
using yoksdotnet.drawing;
using yoksdotnet.drawing.painters;

using Button = System.Windows.Controls.Button;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace yoksdotnet.windows;

public record UndoStep(PaletteIndex Index, RgbColor PreviousColor);

public partial class PaletteCustomizer : Window
{
    private readonly RandomPaletteGenerator _randomPaletteGenerator = new(new());
    private readonly SpriteEditPreviewPainter _previewPainter = new(RefinedBitmap.Neutral);

    private readonly List<UndoStep> _undoStack = [];

    public PaletteCustomizer(CustomPaletteSet set)
    {
        InitializeComponent();

        ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.SelectedEntry))
            {
                OnSelectedPaletteChanged();
            }

            if (new[] { nameof(ViewModel.Hue), nameof(ViewModel.Saturation), nameof(ViewModel.Lightness) }.Contains(e.PropertyName))
            {
                ViewModel.UpdatePalette();
                RefreshSurfaces();
            }
        };

        KeyUp += (s, e) =>
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.Z))
            {
                OnUndo();
            }
        };

        foreach (var entry in set.Entries)
        {
            ViewModel.AddPaletteView(entry.Name, new PaletteView(entry.Palette));
        }

        ViewModel.PaletteToAdd = ViewModel.PredefinedPalettes.First();
        ViewModel.SetName = set.Name;
        ViewModel.SetId = set.Id;

        ViewModel.SelectedEntry = ViewModel.PaletteEntries.FirstOrDefault();

        StartLoop();
    }

    public CustomPaletteSet EditedPaletteGroup 
    { 
        get
        {
            var entries = ViewModel.PaletteEntries
                .Select(entry => new CustomPaletteEntry(entry.Name, entry.Palette))
                .ToList();

            return new(ViewModel.SetId, ViewModel.SetName, entries);
        } 
    }

    protected void StartLoop()
    {
        var loopTimer = new System.Timers.Timer(1000.0 / 60.0);
        loopTimer.Elapsed += (s, e) => PreviewCanvas.Child?.Invalidate();
        loopTimer.Start();
    }

    protected void OnSave(object _sender, RoutedEventArgs _e)
    {
        DialogResult = true;
    }

    protected void OnAddPalette(object _sender, RoutedEventArgs _e)
    {
        var name = WithDuplicateSuffix(ViewModel.PaletteToAdd.Key);
        var palette = ViewModel.PaletteToAdd.Value;

        var paletteCopy = PaletteConversion.Copy(palette.BackingPalette);
        var newEntry = ViewModel.AddPaletteView(name, new(paletteCopy));
        ViewModel.SelectedEntry = newEntry;
    }

    protected void OnAddRandomPalette(object _sender, RoutedEventArgs _e)
    {
        var palette = _randomPaletteGenerator.Generate(1).First();

        var newEntry = ViewModel.AddPaletteView(_randomPaletteGenerator.NamePalette(palette), new(palette));
        ViewModel.SelectedEntry = newEntry;
    }

    protected void OnDeletePalette(object sender, RoutedEventArgs _e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not string paletteId)
        {
            return;
        }

        var paletteName = ViewModel.GetEntryWithId(paletteId).Name;

        var shouldSkipPrompt = Keyboard.Modifiers == ModifierKeys.Shift;
        var shouldDoDelete = shouldSkipPrompt;

        if (!shouldSkipPrompt)
        {
            var result = System.Windows.MessageBox.Show
            (
                $"Are you sure you want to delete '{paletteName}'?\n\n(Hint: hold Shift while deleting to avoid this message)", 
                "Confirm deletion", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            shouldDoDelete = result == MessageBoxResult.Yes;
        }

        if (shouldDoDelete)
        {
            ViewModel.PaletteEntries.Remove(ViewModel.PaletteEntries.Single(e => e.Id == paletteId));
            ViewModel.SelectedEntry = ViewModel.PaletteEntries.FirstOrDefault();
        }
    }
    
    protected void OnCopyPalette(object sender, RoutedEventArgs _e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (button.Tag is not string paletteId)
        {
            return;
        }

        var thisPaletteEntry = ViewModel.PaletteEntries.Single(e => e.Id == paletteId);
        var paletteCopy = PaletteConversion.Copy(thisPaletteEntry.Palette);

        var newName = WithDuplicateSuffix(thisPaletteEntry.Name);
        var newEntry = ViewModel.AddPaletteView(newName, new(paletteCopy));
        ViewModel.SelectedEntry = newEntry;
    }

    private void OnSelectedPaletteChanged()
    {
        ViewModel.UpdateHsl();
        RefreshSurfaces();

        _undoStack.Clear();
    }

    private void RefreshSurfaces()
    {
        PreviewCanvas.Child?.Invalidate();
        ColorSelectCanvas.Child?.Invalidate();
    }

    private void OnPaintPreview(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        var doMouseUpdate = ViewModel.SelectedEntry?.Name != "Loxxe";

        if (doMouseUpdate)
        {
            var screenMouse = Control.MousePosition;
            var mouse = PreviewCanvas.PointFromScreen(new(screenMouse.X, screenMouse.Y));

            _previewPainter.MousePos = new(mouse.X, mouse.Y);
        }

        _previewPainter.Draw(e.Surface.Canvas, ViewModel.SelectedEntry?.Palette);
    }

    private void OnPaintPreviewMouseUp(object? sender, MouseEventArgs e)
    {
        if (ViewModel.SelectedEntry is null)
        {
            return;
        }

        if (_previewPainter.HoveredIndex is null)
        {
            return;
        }

        ViewModel.SelectedIndex = _previewPainter.HoveredIndex;

        ViewModel.UpdateHsl();
        RefreshSurfaces();
    }

    private void OnPreviewMouseMove(object? sender, MouseEventArgs e)
    {
        if (ViewModel.SelectedEntry is null)
        {
            return;
        }

        if (sender is not SKGLControl glControl)
        {
            return;
        }

        _previewPainter.UpdateHoveredIndex(e.X, e.Y, glControl.Width, glControl.Height);

        RefreshSurfaces();
    }

    private void OnEntryKeyboardFocused(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (textBox.Tag is not string entryId)
        {
            return;
        }

        ViewModel.SelectedEntry = ViewModel.GetEntryWithId(entryId);
    }

    private void OnPaintColorSelect(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        var ctx = e.Surface.Canvas;

        if (ViewModel.SelectedEntry is null)
        {
            ctx.Clear();
            return;
        }

        ColorSelectPainter.Draw(ctx, new(ViewModel.Hue, ViewModel.Saturation, ViewModel.Lightness));
    }

    private void OnColorSelectMouseDown(object? sender, MouseEventArgs e)
    {
        if (ViewModel.SelectedEntry is not { } entry)
        {
            return;
        }

        if (sender is not SKGLControl glControl)
        {
            return;
        }

        var index = ViewModel.SelectedIndex;
        UpdateUndoable(index, entry.Palette[index]);

        OnColorSelectMouseMove(sender, e);
    }

    private void OnColorSelectMouseMove(object? sender, MouseEventArgs e)
    {
        if (ViewModel.SelectedEntry is null)
        {
            return;
        }

        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        if (sender is not SKGLControl glControl)
        {
            return;
        }

        var (saturation, lightness) = SlXyConverter.CoordToSl(e.X, e.Y, glControl.Width, glControl.Height);

        ViewModel.Saturation = saturation;
        ViewModel.Lightness = lightness;
    }

    private SKGLControl InitCanvas(WindowsFormsHost host, EventHandler<SKPaintGLSurfaceEventArgs> handler)
    {
        var glControl = new SKGLControl();
        glControl.PaintSurface += handler;
        glControl.Dock = DockStyle.Fill;

        host.Child = glControl;

        return glControl;
    }

    private void InitPreviewCanvas(object? sender, EventArgs e)
    {
        if (sender is not WindowsFormsHost host)
        {
            return;
        }

        var glControl = InitCanvas(host, OnPaintPreview);
        glControl.MouseMove += OnPreviewMouseMove;
        glControl.MouseUp += OnPaintPreviewMouseUp;
    }

    private void InitColorSelectCanvas(object? sender, EventArgs e)
    {
        if (sender is not WindowsFormsHost host)
        {
            return;
        }

        var glControl = InitCanvas(host, OnPaintColorSelect);
        glControl.MouseMove += OnColorSelectMouseMove;
        glControl.MouseDown += OnColorSelectMouseDown;
    }

    private void OnExport(object _sender, RoutedEventArgs _e)
    {
        if (ViewModel.SelectedEntry is not { } entry)
        {
            return;
        }

        var folderDialog = new OpenFolderDialog();
        if (folderDialog.ShowDialog() is not true)
        {
            return;
        }

        var exportPath = Path.Combine(folderDialog.FolderName, entry.Name);

        var imageExporter = new ImageExporter(exportPath);
        var exportBitmaps = (List<Bitmap>) [
            ..SfEnums.GetAll<ClassicBitmap>().Select(Bitmap.Classic),
            ..SfEnums.GetAll<RefinedBitmap>().Select(Bitmap.Refined)
        ];
        var exportResults = exportBitmaps.Select(b => imageExporter.Export(b, entry.Palette));

        if (exportResults.All(r => r == ImageExportResult.Ok))
        {
            Process.Start("explorer.exe", exportPath);
        }
        else if (exportResults.Any(r => r == ImageExportResult.NoPermission))
        {
            System.Windows.MessageBox.Show
            (
                "It seems I don't have permission to write files here. Try somewhere else?",
                "Export error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
        else
        {
            System.Windows.MessageBox.Show
            (
                "Something went wrong trying to write files. Check that the folder you're trying to write to still exists.",
                "Export error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    private void OnShare(object _sender, RoutedEventArgs _e)
    {
        var shareDialog = new PaletteExportDialog(EditedPaletteGroup);
        shareDialog.ShowDialog();
    }

    private void OnManualHexChange(object sender, System.Windows.Input.KeyEventArgs _e)
    {
        if (sender is not TextBox textBox)
        {
            return;
        }

        if (ViewModel.SelectedEntry is not { } entry) 
        {
            return;
        }

        var hex = textBox.Text;
        ViewModel.CurrentHex = hex;

        var inputColor = ColorConversion.FromHex(hex);
        if (inputColor is null)
        {
            return;
        }

        UpdateUndoable(ViewModel.SelectedIndex, inputColor.Value);
    }

    private void OnSmartFill(object _sender, RoutedEventArgs _e)
    {
        if (ViewModel.SelectedEntry is not { } entry) 
        {
            return;
        }

        var selectedIndex = ViewModel.SelectedIndex;

        if (selectedIndex == PaletteIndex.Scales)
        {
            var fromShadow = ColorManipulation.LightenColor(entry.Palette.hornsShadow);
            var fromHighlight = ColorManipulation.DarkenColor(entry.Palette.scalesHighlight);
            var average = ColorManipulation.ColorBetween(entry.Palette.scalesShadow, entry.Palette.scalesHighlight);

            var fromShadowDelta = ColorManipulation.Distance(fromShadow, entry.Palette.scalesShadow);
            var fromHighlightDelta = ColorManipulation.Distance(fromHighlight, entry.Palette.scalesHighlight);
            var averageDelta = ColorManipulation.Distance(average, entry.Palette.scalesShadow);

            var deltaMax = new[] { fromShadowDelta, fromHighlightDelta, averageDelta }.Max();

            var newColor = deltaMax == fromShadowDelta ? fromShadow
                : deltaMax == fromHighlightDelta ? fromHighlight
                : average;

            UpdateUndoable(PaletteIndex.Scales, newColor);
        }

        if (selectedIndex == PaletteIndex.ScalesHighlight)
        {
            var newColor = ColorManipulation.LightenColor(entry.Palette.scales);
            UpdateUndoable(PaletteIndex.ScalesHighlight, newColor);
        }

        if (selectedIndex == PaletteIndex.ScalesShadow)
        {
            var newColor = ColorManipulation.DarkenColor(entry.Palette.scales);
            UpdateUndoable(PaletteIndex.ScalesShadow, newColor);
        }

        if (selectedIndex == PaletteIndex.HornsShadow)
        {
            var newColor = ColorManipulation.DarkenColor(entry.Palette.horns);
            UpdateUndoable(PaletteIndex.HornsShadow, newColor);
        }

        if (selectedIndex == PaletteIndex.Horns)
        {
            var newColor = ColorManipulation.LightenColor(entry.Palette.hornsShadow);
            UpdateUndoable(PaletteIndex.HornsShadow, newColor);
        }

        if (selectedIndex == PaletteIndex.Whites)
        {
            var newColor = ColorManipulation.WhitenColor(entry.Palette.scales);
            UpdateUndoable(PaletteIndex.Whites, newColor);
        }

        if (selectedIndex == PaletteIndex.Eyes)
        {
            var newColor = ColorManipulation.DarkenColor(entry.Palette.scales);
            UpdateUndoable(PaletteIndex.Eyes, newColor);
        }
    }

    private void UpdateUndoable(PaletteIndex index, RgbColor color)
    {
        if (ViewModel.SelectedEntry is not { } entry) 
        {
            return;
        }

        var newStep = new UndoStep(index, entry.Palette[index]);
        _undoStack.Insert(0, newStep);

        entry.Palette[index] = color;

        ViewModel.UpdateHex();
        ViewModel.UpdateHsl();
    }

    private void OnUndo()
    {
        if (ViewModel.SelectedEntry is not { } entry) 
        {
            return;
        }

        var undoStep = _undoStack.FirstOrDefault();
        if (undoStep is null)
        {
            return;
        }

        entry.Palette[undoStep.Index] = undoStep.PreviousColor;
        ViewModel.UpdateHsl();
        ViewModel.UpdateHex();

        _undoStack.RemoveAt(0);
    }

    private string WithDuplicateSuffix(string name)
    {
        var needsSuffix = ViewModel.PaletteEntries
            .Select(e => e.Name)
            .Contains(name);

        if (!needsSuffix)
        {
            return name;
        }

        return WithDuplicateSuffix(NextDuplicateName(name));
    }

    private string NextDuplicateName(string name)
    {
        var matchResult = Regex.Match(name, @"^(.+?)\s*\((\d+)\)$");

        var baseName = matchResult.Success
            ? matchResult.Groups[1].Value
            : name;

        var number = matchResult.Success
            ? int.Parse(matchResult.Groups[2].Value)
            : 1;

        return $"{baseName} ({number + 1})";
    }
}

public class PaletteCustomizerViewModel : INotifyPropertyChanged
{
    public string SetId { get; set; } = null!;

    private string _setName = "New palette set";
    public string SetName
    {
        get => _setName.Trim();
        set
        {
            _setName = value.Trim();
            OnPropertyChanged(nameof(SetName));
        }
    }

    public Dictionary<string, PaletteView> PredefinedPalettes { get; init; } = SfEnums.GetAll<PredefinedPalette>()
        .Select(p => (p.Name, new PaletteView(p)))
        .Prepend(("(template)", new PaletteView(PaletteConversion.Copy(Palette.ExamplePalette))))
        .ToDictionary();

    private KeyValuePair<string, PaletteView> _paletteToAdd;
    public KeyValuePair<string, PaletteView> PaletteToAdd
    {
        get => _paletteToAdd;
        set
        {
            _paletteToAdd = value;
            OnPropertyChanged(nameof(PaletteToAdd));
        }
    }

    private ObservableCollection<PaletteViewEntry> _paletteEntries = new();
    public ObservableCollection<PaletteViewEntry> PaletteEntries { 
        get => _paletteEntries;
        set 
        {
            _paletteEntries = value;
            OnPropertyChanged(nameof(PaletteEntries));
        }
    }

    private PaletteViewEntry? _selectedEntry;
    public PaletteViewEntry? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            _selectedEntry = value;
            OnPropertyChanged(nameof(SelectedEntry));
            OnPropertyChanged(nameof(IsPaletteSelected));
            OnPropertyChanged(nameof(HueStripVisibility));
        }
    }

    public PaletteViewEntry GetEntryWithId(string id)
    {
        var paletteView = PaletteEntries.First(e => e.Id == id);
        return paletteView;
    }

    public bool IsPaletteSelected => SelectedEntry is not null;
    public Visibility HueStripVisibility => SelectedEntry is null 
        ? Visibility.Hidden 
        : Visibility.Visible;

    private PaletteIndex _selectedIndex = PaletteIndex.Scales;
    public PaletteIndex SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            OnPropertyChanged(nameof(SelectedIndex));
        }
    }

    private float _hue = 0;
    public float Hue
    {
        get => _hue;
        set
        {
            _hue = value;
            OnPropertyChanged(nameof(Hue));
            UpdateHex();
        }
    }

    private float _saturation = 0;
    public float Saturation
    {
        get => _saturation;
        set
        {
            _saturation = value;
            OnPropertyChanged(nameof(Saturation));
            UpdateHex();
        }
    }

    private float _lightness = 0;
    public float Lightness
    {
        get => _lightness;
        set
        {
            _lightness = value;
            OnPropertyChanged(nameof(Lightness));
            UpdateHex();
        }
    }

    private string _currentHex = "";
    public string CurrentHex
    {
        get => _currentHex;
        set
        {
            _currentHex = value;
            OnPropertyChanged(nameof(CurrentHex));
        }
    }

    public bool IsValidHex => ColorConversion.FromHex(CurrentHex) is not null;

    public void UpdateHsl()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        var (h, s, l) = ColorConversion.ToHsl(SelectedEntry.PaletteView[SelectedIndex]);

        Hue = (float)h;
        Saturation = (float)s;
        Lightness = (float)l;
    }

    public void UpdateHex()
    {
        CurrentHex = ColorConversion.ToHex(ColorConversion.FromHsl(new(Hue, Saturation, Lightness)));
    }

    public void UpdatePalette()
    {
        if (SelectedEntry is { } selectedEntry)
        {
            selectedEntry.PaletteView[SelectedIndex] = ColorConversion.FromHsl(new(Hue, Saturation, Lightness));
            OnPropertyChanged(nameof(PaletteEntries));
        }
    }

    public PaletteViewEntry AddPaletteView(string name, PaletteView paletteView)
    {
        var newEntry = new PaletteViewEntry(Guid.NewGuid().ToString(), name, paletteView);

        PaletteEntries.Add(newEntry);

        return newEntry;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public class PaletteViewEntry(string id, string name, PaletteView paletteView) 
{
    public string Id => id;

    private string _name = name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public PaletteView PaletteView => paletteView;
    public Palette Palette => PaletteView.BackingPalette;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
