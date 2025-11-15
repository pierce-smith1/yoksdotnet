using SkiaSharp;
using SkiaSharp.Views.Desktop;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using yoksdotnet.drawing;
using yoksdotnet.logic.scene;

namespace yoksdotnet.windows;

public class PreviewSprite : Sprite
{
    public required Bitmap Bitmap { get; set; }
}

public partial class PaletteCustomizer : Window
{
    private readonly EntityPainter _spritePainter;
    private readonly SKPaint _currentPaint;
    private readonly PreviewSprite _currentSprite;

    public readonly Palette GhostPalette = new(new(
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#ffffff",
        "#aaaaaa"
    ));

    public PaletteCustomizer()
    {
        InitializeComponent();

        _spritePainter = new EntityPainter();
        _currentPaint = ViewModel.Palettes.Values.FirstOrDefault()?.Paint ?? GhostPalette.Paint;
        _currentSprite = new PreviewSprite()
        {
            Id = 0,
            Brand = 0,
            Home = new(),
            Offset = new(),
            Scale = 1,
            Width = Bitmap.BitmapSize(),
            Height = Bitmap.BitmapSize(),
            AngleRadians = 0.0,
            Paint = _currentPaint,
            Bitmap = Bitmap.LkSix,
        };

        RefreshSurface();
    }

    private void RefreshSurface()
    {
        PreviewCanvas.Child?.Invalidate();
    }

    private void OnPaintSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        _currentSprite.Home.X = (e.Surface.Canvas.LocalClipBounds.Width / 2) - (Bitmap.BitmapSize() / 2);
        _currentSprite.Home.Y = (e.Surface.Canvas.LocalClipBounds.Height / 2) - (Bitmap.BitmapSize() / 2);

        e.Surface.Canvas.Clear(new SKColor(0x11, 0x11, 0x11));

        _spritePainter.Draw(e.Surface.Canvas, _currentSprite);
    }

    private void CreateGlContext(object? sender, EventArgs e)
    {
        var glControl = new SKGLControl();
        glControl.PaintSurface += OnPaintSurface;
        glControl.Dock = DockStyle.Fill;

        var host = (WindowsFormsHost)sender!;
        host.Child = glControl;
    }
}

public class PaletteCustomizerViewModel : INotifyPropertyChanged
{
    private Dictionary<string, Palette> _palettes { get; set; } = new() {
        { "Chasnah", new(new(
            "#6f31dd",
            "#932de3",
            "#3a12a2",
            "#e30efe",
            "#e227ff",
            "#f6f5f4",
            "#e959f5"
        ))},
        { "Ellai", new(new(
            "#97cc72",
            "#caecb1",
            "#338527",
            "#72482d",
            "#f04f2a",
            "#ffffff",
            "#45160e"
        ))},
    };

    public Dictionary<string, Palette> Palettes { 
        get => _palettes;
        set {
            _palettes = value;
            OnPropertyChanged(nameof(Palettes));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
