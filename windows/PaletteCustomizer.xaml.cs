using SkiaSharp;
using SkiaSharp.Views.Desktop;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using yoksdotnet.common;
using yoksdotnet.drawing;
using yoksdotnet.logic.scene;

using Point = (int X, int Y);

namespace yoksdotnet.windows;

public class PreviewSprite : Sprite
{
    public required Bitmap Bitmap { get; set; }
    public required Palette Palette { get; set; }
    public PaletteIndex? HoveredIndex { get; set; }

    public override Bitmap GetBitmap() => Bitmap;
    public override Palette GetPalette()
    {
        if (HoveredIndex is null)
        {
            return Palette;
        }

        var newColors = HoveredIndex switch
        {
            _ when HoveredIndex == PaletteIndex.Scales => Palette.Colors with { Scales = ToHoveredColor(Palette.Colors.Scales) },
            _ when HoveredIndex == PaletteIndex.ScalesHighlight => Palette.Colors with { ScalesHighlight = ToHoveredColor(Palette.Colors.ScalesHighlight)},
            _ when HoveredIndex == PaletteIndex.ScalesShadow => Palette.Colors with { ScalesShadow = ToHoveredColor(Palette.Colors.ScalesShadow) },
            _ when HoveredIndex == PaletteIndex.Horns => Palette.Colors with { Horns = ToHoveredColor(Palette.Colors.Horns)},
            _ when HoveredIndex == PaletteIndex.HornsShadow => Palette.Colors with { HornsShadow = ToHoveredColor(Palette.Colors.HornsShadow)},
            _ when HoveredIndex == PaletteIndex.Eyes => Palette.Colors with { Eyes = ToHoveredColor(Palette.Colors.Eyes) },
            _ when HoveredIndex == PaletteIndex.Whites => Palette.Colors with { Whites = ToHoveredColor(Palette.Colors.Whites) },
            _ => throw new Exception(),
        };
        return new Palette(newColors);
    }

    private Color ToHoveredColor(Color c)
    {
        const int lightenAmount = 100;

        var newColor = new Color(
            (byte)Math.Clamp(c.R + lightenAmount, 0, 255),
            (byte)Math.Clamp(c.G + lightenAmount * 2, 0, 255),
            (byte)Math.Clamp(c.B + lightenAmount, 0, 255)
        );
        
        return newColor;
    }
}

public partial class PaletteCustomizer : Window
{
    private readonly EntityPainter _spritePainter;
    private readonly PreviewSprite _currentSprite;

    public readonly Palette GhostPalette = new(new(
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#aaaaaa",
        "#cccccc",
        "#aaaaaa"
    ));

    private SKRuntimeEffect _colorSelectShader;
    private SKRuntimeEffectUniforms _colorSelectShaderUniforms;
    private Dictionary<PaletteIndex, List<Point>> _paletteIndexRegions;

    public PaletteCustomizer()
    {
        InitializeComponent();

        _spritePainter = new EntityPainter();
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
            Palette = ViewModel.SelectedPaletteEntry?.Value ?? GhostPalette,
            Bitmap = Bitmap.LkThumbsup,
        };

        _paletteIndexRegions = GetPaletteIndexRegionsFrom(Bitmap.LkThumbsup.Resource);

        ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ViewModel.SelectedPaletteEntry))
            {
                OnSelectedPaletteChanged();
            }
            else if (e.PropertyName == nameof(ViewModel.Hue)
                || e.PropertyName == nameof(ViewModel.Saturation)
                || e.PropertyName == nameof(ViewModel.Lightness)
            ) {
                RefreshSurfaces();
            }
        };

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
        _currentSprite.Palette = ViewModel.SelectedPaletteEntry?.Value ?? GhostPalette;

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
        if (_currentSprite.HoveredIndex is null)
        {
            return;
        }

        ViewModel.SelectedPaletteIndex = _currentSprite.HoveredIndex;
    }

    private void OnPreviewMouseMove(object? sender, MouseEventArgs e)
    {
        var glControl = sender as SKGLControl ?? throw new InvalidOperationException();
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

        _colorSelectShaderUniforms["resolution"] = new float[] {e.Info.Width, e.Info.Height};
        _colorSelectShaderUniforms["hue"] = ViewModel.Hue;
        var selectPaint = new SKPaint()
        {
            Shader = _colorSelectShader.ToShader(isOpaque: true, uniforms: _colorSelectShaderUniforms),
        };

        ctx.DrawRect(0, 0, e.Info.Width, e.Info.Height, selectPaint);

        if (!IsAnyPaletteSelected())
        {
            return;
        }

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

    private void OnColorSelectMouseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        var glControl = sender as SKGLControl ?? throw new InvalidOperationException();

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
        if (sender is WindowsFormsHost host)
        {
            var glControl = InitCanvas(host, OnPaintPreview);
            glControl.MouseMove += OnPreviewMouseMove;
        }
        else throw new InvalidOperationException();
    }

    private void InitColorSelectCanvas(object? sender, EventArgs e)
    {
        if (sender is WindowsFormsHost host)
        {
            var glControl = InitCanvas(host, OnPaintColorSelect);
            glControl.MouseMove += OnColorSelectMouseMove;
        }
        else throw new InvalidOperationException();
    }
    
    private bool IsAnyPaletteSelected()
    {
        return ViewModel.SelectedPaletteEntry is not null;
    }
}

public class PaletteCustomizerViewModel : INotifyPropertyChanged
{
    private Dictionary<string, Palette> _palettes = new() {
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
        set 
        {
            _palettes = value;
            OnPropertyChanged(nameof(Palettes));
        }
    }

    private KeyValuePair<string, Palette>? _selectedPaletteEntry;
    public KeyValuePair<string, Palette>? SelectedPaletteEntry
    {
        get => _selectedPaletteEntry;
        set
        {
            _selectedPaletteEntry = value;
            OnPropertyChanged(nameof(SelectedPaletteEntry));
        }
    }

    private PaletteIndex _selectedPaletteIndex = PaletteIndex.Scales;
    public PaletteIndex SelectedPaletteIndex
    {
        get => _selectedPaletteIndex;
        set
        {
            _selectedPaletteIndex = value;
            OnPropertyChanged(nameof(SelectedPaletteIndex));
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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
