using System.ComponentModel;
using yoksdotnet.drawing;

using RgbColor = yoksdotnet.drawing.RgbColor;

namespace yoksdotnet.windows;

public class PaletteView(Palette _backingPalette) : INotifyPropertyChanged
{
    public Palette BackingPalette => _backingPalette;

    public RgbColor Scales
    {
        get => _backingPalette.Scales;
        set 
        {
            _backingPalette.Scales = value;
            OnPropertyChanged(nameof(Scales));
            OnPropertyChanged(nameof(ScalesHex));
        }
    }

    public RgbColor ScalesHighlight
    {
        get => _backingPalette.ScalesHighlight;
        set 
        {
            _backingPalette.ScalesHighlight = value;
            OnPropertyChanged(nameof(ScalesHighlight));
            OnPropertyChanged(nameof(ScalesHighlightHex));
        }
    }

    public RgbColor ScalesShadow
    {
        get => _backingPalette.ScalesShadow;
        set 
        {
            _backingPalette.ScalesShadow = value;
            OnPropertyChanged(nameof(ScalesShadow));
            OnPropertyChanged(nameof(ScalesShadowHex));
        }
    }

    public RgbColor Horns
    {
        get => _backingPalette.Horns;
        set 
        {
            _backingPalette.Horns = value;
            OnPropertyChanged(nameof(Horns));
            OnPropertyChanged(nameof(HornsHex));
        }
    }

    public RgbColor Eyes
    {
        get => _backingPalette.Eyes;
        set 
        {
            _backingPalette.Eyes = value;
            OnPropertyChanged(nameof(Eyes));
            OnPropertyChanged(nameof(EyesHex));
        }
    }

    public RgbColor Whites
    {
        get => _backingPalette.Whites;
        set
        {
            _backingPalette.Whites = value;
            OnPropertyChanged(nameof(Whites));
            OnPropertyChanged(nameof(WhitesHex));
        }
    }

    public RgbColor HornsShadow
    {
        get => _backingPalette.HornsShadow;
        set
        {
            _backingPalette.HornsShadow = value;
            OnPropertyChanged(nameof(HornsShadow));
            OnPropertyChanged(nameof(HornsShadowHex));
        }
    }

    public RgbColor this[PaletteIndex index]
    {
        get => _backingPalette[index];
        set
        {
            _backingPalette[index] = value;
            OnPropertyChanged(index.Name);
            OnPropertyChanged($"{index.Name}Hex");
        }
    }
    public string ScalesHex => Scales.ToHex();
    public string ScalesHighlightHex => ScalesHighlight.ToHex();
    public string ScalesShadowHex => ScalesShadow.ToHex();
    public string HornsHex => Horns.ToHex();
    public string EyesHex => Eyes.ToHex();
    public string WhitesHex => Whites.ToHex();
    public string HornsShadowHex => HornsShadow.ToHex();

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
