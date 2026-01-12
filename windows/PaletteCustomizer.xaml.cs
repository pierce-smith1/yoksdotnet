using Microsoft.Win32;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using yoksdotnet.common;
using yoksdotnet.drawing;
using yoksdotnet.logic;
using yoksdotnet.logic.scene;

using Button = System.Windows.Controls.Button;
using Point = (int X, int Y);
using TextBox = System.Windows.Controls.TextBox;

namespace yoksdotnet.windows;

public class PreviewSprite : Sprite
{
    public required Bitmap Bitmap { get; set; }
    public required Palette Palette { get; set; }
    public PaletteIndex? HoveredIndex { get; set; }

    public override Bitmap GetBitmap() => Bitmap;
    public override SKPaint GetPaint() 
    {
        if (HoveredIndex is null)
        {
            return Palette.GetPaint();
        }

        var hoveredPalette = new Palette(Palette);
        hoveredPalette[HoveredIndex] = ToHoveredColor(Palette[HoveredIndex]);

        return hoveredPalette.GetPaint();
    }

    private Color ToHoveredColor(Color c)
    {
        const int lightenAmount = 100;

        var newColor = new Color(
            (byte)Math.Clamp(c.R + lightenAmount, 0, 255),
            (byte)Math.Clamp(c.G + lightenAmount, 0, 255),
            (byte)Math.Clamp(c.B + lightenAmount, 0, 255)
        );
        
        return newColor;
    }
}

public partial class PaletteCustomizer : Window
{
    private readonly EntityPainter _spritePainter;
    private readonly PreviewSprite _currentSprite;
    private readonly OptionsSaver _optionsSaver = new();

    public readonly Palette GhostPalette = new(
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#cccccc",
        "#aaaaaa"
    );

    private readonly SKRuntimeEffect _colorSelectShader;
    private readonly SKRuntimeEffectUniforms _colorSelectShaderUniforms;
    private readonly Dictionary<PaletteIndex, List<Point>> _paletteIndexRegions;

    public PaletteCustomizer(List<CustomPaletteEntry> customPalettes)
    {
        InitializeComponent();

        _spritePainter = new EntityPainter();
        _currentSprite = new PreviewSprite()
        {
            Id = 0,
            Brand = 0,
            Home = new(0, 0),
            Offset = new(0, 0),
            Scale = 1,
            Width = Bitmap.BitmapSize(),
            Height = Bitmap.BitmapSize(),
            AngleRadians = 0.0,
            Palette = ViewModel.SelectedId is { } paletteId 
                ? ViewModel.GetEntryWithId(paletteId).Palette
                : GhostPalette,
            Bitmap = Bitmap.LkThumbsup,
        };

        _paletteIndexRegions = GetPaletteIndexRegionsFrom(_currentSprite.Bitmap.Resource);

        ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.SelectedId))
            {
                OnSelectedPaletteChanged();
            }

            if (new[] { nameof(ViewModel.Hue), nameof(ViewModel.Saturation), nameof(ViewModel.Lightness) }.Contains(e.PropertyName))
            {

                ViewModel.UpdatePalette();
                RefreshSurfaces();
            }
        };

        foreach (var entry in customPalettes)
        {
            ViewModel.AddPaletteView(entry.Name, new PaletteView(entry.Palette));
        }

        ViewModel.PaletteToAdd = ViewModel.PredefinedPalettes.First();
        ViewModel.SelectedEntry = ViewModel.PaletteEntries.FirstOrDefault();

        _colorSelectShader = SKRuntimeEffect.Create(@"
            uniform half2 resolution;
            uniform half hue;

            half mod(half a, half b) {
                return a - b * floor(a / b);
            }

            half4 coord_to_rgb(vec2 coord) {
                half h = hue;
                half l = coord.x;
                half s = coord.y;

                half c = (1 - abs(2.0 * l - 1)) * s;
                half x = c * (1 - abs(mod(h / 60, 2.0) - 1));
                half m = l - c / 2.0;

                if (h < 60.0) {
                    return half4(c + m, x + m, m, 1.0);
                } else if (h < 120.0) {
                    return half4(x + m, c + m, m, 1.0);
                } else if (h < 180.0) {
                    return half4(m, c + m, x + m, 1.0);
                } else if (h < 240.0) {
                    return half4(m, x + m, c + m, 1.0);
                } else if (h < 300.0) {
                    return half4(x + m, m, c + m, 1.0);
                } else {
                    return half4(c + m, m, x + m, 1.0);
                }
            }

            half4 main(vec2 coord) {
                half4 rgb = coord_to_rgb(coord / resolution);
                return half4(rgb.r, rgb.g, rgb.b, 1.0);
            }
        ", out var _errorText);

        _colorSelectShaderUniforms = new(_colorSelectShader);

        RefreshSurfaces();
    }

    public List<CustomPaletteEntry> GetPaletteState()
    {
        return ViewModel.PaletteEntries
            .Select(entry => new CustomPaletteEntry(entry.Name, entry.Palette))
            .ToList();
    }

    protected void OnSave(object? _sender, RoutedEventArgs _e)
    {
        _optionsSaver.SaveCustomPalettes(GetPaletteState());
        DialogResult = true;
    }

    protected void OnCancel(object? _sender, RoutedEventArgs _e)
    {
        DialogResult = false;
    }

    protected void OnAddPalette(object? _sender, RoutedEventArgs _e)
    {
        var name = ViewModel.PaletteToAdd.Key;
        var palette = ViewModel.PaletteToAdd.Value;

        var newEntry = ViewModel.AddPaletteView(name, palette);
        ViewModel.SelectedEntry = newEntry;
    }

    protected void OnDeletePalette(object? sender, RoutedEventArgs _e)
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

        var result = System.Windows.MessageBox.Show
        (
            $"Are you sure you want to delete '{paletteName}'?", 
            "Confirm deletion", 
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (result == MessageBoxResult.Yes)
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
        var paletteCopy = new Palette(thisPaletteEntry.Palette);

        var newEntry = ViewModel.AddPaletteView(thisPaletteEntry.Name, new(paletteCopy));
        ViewModel.SelectedEntry = newEntry;
    }

    private Dictionary<PaletteIndex, List<Point>> GetPaletteIndexRegionsFrom(SKBitmap bitmap)
    {
        var indexes = StaticFieldEnumerations.GetAll<PaletteIndex>();

        var regions = new Dictionary<PaletteIndex, List<Point>>();

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (color.Alpha != 255)
                {
                    continue;
                }

                var index = indexes.FirstOrDefault(i => i.Luminance == color.Red);
                if (index is null)
                {
                    continue;
                }

                regions.TryAdd(index, []);
                regions[index].Add((x, y));
            }
        }

        return regions;
    }

    private void OnSelectedPaletteChanged()
    {
        if (ViewModel.SelectedId is { } entryId)
        {
            _currentSprite.Palette = ViewModel.GetEntryWithId(entryId).Palette;
        }
        else
        {
            _currentSprite.Palette = GhostPalette;
        }

        ViewModel.UpdateHsl();
        RefreshSurfaces();
    }

    private void RefreshSurfaces()
    {
        PreviewCanvas.Child?.Invalidate();
        ColorSelectCanvas.Child?.Invalidate();
    }

    private void OnPaintPreview(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        var ctx = e.Surface.Canvas;

        _currentSprite.Home.X = (ctx.LocalClipBounds.Width / 2) - (Bitmap.BitmapSize() / 2);
        _currentSprite.Home.Y = (ctx.LocalClipBounds.Height / 2) - (Bitmap.BitmapSize() / 2);

        ctx.Clear(new SKColor(0x11, 0x11, 0x11));

        _spritePainter.Draw(ctx, _currentSprite);
    }

    private void OnPaintPreviewMouseUp(object? sender, MouseEventArgs e)
    {
        if (ViewModel.SelectedId is null)
        {
            return;
        }

        if (_currentSprite.HoveredIndex is null)
        {
            return;
        }

        ViewModel.SelectedIndex = _currentSprite.HoveredIndex;

        ViewModel.UpdateHsl();
        RefreshSurfaces();
    }

    private void OnPreviewMouseMove(object? sender, MouseEventArgs e)
    {
        if (ViewModel.SelectedId is null)
        {
            return;
        }

        if (sender is not SKGLControl glControl)
        {
            return;
        }

        var pos = (e.X, e.Y);

        foreach (var (index, points) in _paletteIndexRegions)
        {
            var offsetPoints = points.Select(p => (
                p.X + glControl.Width / 2 - Bitmap.BitmapSize() / 2,
                p.Y + glControl.Height / 2 - Bitmap.BitmapSize() / 2
            ));

            if (offsetPoints.Contains(pos))
            {
                _currentSprite.HoveredIndex = index;
                RefreshSurfaces();
                return;
            }
        }

        _currentSprite.HoveredIndex = null;
        RefreshSurfaces();
    }

    private void OnEntryKeyboardFocused(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
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

    private (float, float) ColorSelectCoordToSl(float x, float y, float width, float height)
    {
        var saturation = (float)Interpolation.InterpLinear(y, 0, height, 0, 100);
        var lightness = (float)Interpolation.InterpLinear(x, 0, width, 0, 100);

        return (saturation, lightness);
    }

    private (float, float) SlToColorSelectCoord(float saturation, float lightness, float width, float height)
    {
        var x = (float)Interpolation.InterpLinear(lightness, 0, 100, 0, width);
        var y = (float)Interpolation.InterpLinear(saturation, 0, 100, 0, height);

        return (x, y);
    }

    private void OnPaintColorSelect(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        var ctx = e.Surface.Canvas;

        if (ViewModel.SelectedEntry is null)
        {
            ctx.Clear();
            return;
        }

        _colorSelectShaderUniforms["resolution"] = new float[] {e.Info.Width, e.Info.Height};
        _colorSelectShaderUniforms["hue"] = ViewModel.Hue;
        var selectPaint = new SKPaint()
        {
            Shader = _colorSelectShader.ToShader(isOpaque: true, uniforms: _colorSelectShaderUniforms),
        };

        ctx.DrawRect(0, 0, e.Info.Width, e.Info.Height, selectPaint);

        const float selectedBoxSize = 30;

        var (selectedX, selectedY) = SlToColorSelectCoord(ViewModel.Saturation, ViewModel.Lightness, e.Info.Width, e.Info.Height);

        var selectedBoxFill = new SKPaint()
        {
            Color = SKColor.FromHsl(ViewModel.Hue, ViewModel.Saturation, ViewModel.Lightness),
        };

        var selectedBoxStroke = new SKPaint()
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Stroke,
        };

        var selectedBoxBounds = SKRect.Create(
            x: selectedX - selectedBoxSize / 2,
            y: selectedY - selectedBoxSize / 2,
            width: selectedBoxSize,
            height: selectedBoxSize
        );

        ctx.DrawRect(selectedBoxBounds, selectedBoxFill);
        ctx.DrawRect(selectedBoxBounds, selectedBoxStroke);
    }

    private void OnColorSelectMouseMove(object? sender, MouseEventArgs e)
    {
        if (ViewModel.SelectedId is null)
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

        var (saturation, lightness) = ColorSelectCoordToSl(e.X, e.Y, glControl.Width, glControl.Height);

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
        foreach (var bitmap in StaticFieldEnumerations.GetAll<Bitmap>())
        {
            var exportResult = imageExporter.Export(bitmap, entry.Palette);

            if (exportResult == ImageExportResult.NoPermission)
            {
                System.Windows.MessageBox.Show
                (
                    "It seems I don't have permission to write files here. Try somewhere else?",
                    "Export error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                break;
            }
        }
    }
}

public class PaletteCustomizerViewModel : INotifyPropertyChanged
{
    public Dictionary<string, PaletteView> PredefinedPalettes { get; init; } = StaticFieldEnumerations.GetAll<PredefinedPalette>()
        .Select(p => (p.Name, new PaletteView(p)))
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
            OnPropertyChanged(nameof(SelectedId));
            OnPropertyChanged(nameof(SelectedName));
            OnPropertyChanged(nameof(IsPaletteSelected));
            OnPropertyChanged(nameof(HueStripVisibility));
        }
    }

    public PaletteViewEntry GetEntryWithId(string id)
    {
        var paletteView = PaletteEntries.First(e => e.Id == id);
        return paletteView;
    }

    public string? SelectedId => SelectedEntry?.Id;
    public string? SelectedName => SelectedEntry?.Name;
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
            OnPropertyChanged(nameof(SelectedIndexName));
        }
    }

    public string SelectedIndexName => SelectedIndex.ToString();

    private float _hue = 0;
    public float Hue
    {
        get => _hue;
        set
        {
            _hue = value;
            OnPropertyChanged(nameof(Hue));
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
        }
    }
    public void UpdateHsl()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        var (h, s, l) = SelectedEntry.PaletteView[SelectedIndex].ToHsl();

        Hue = (float)h;
        Saturation = (float)s;
        Lightness = (float)l;
    }

    public void UpdatePalette()
    {
        if (SelectedEntry is { } selectedEntry)
        {
            selectedEntry.PaletteView[SelectedIndex] = Color.FromHsl(Hue, Saturation, Lightness);
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

public record PaletteViewEntry(string Id, string Name, PaletteView PaletteView) 
{
    public Palette Palette => PaletteView.BackingPalette;
}
