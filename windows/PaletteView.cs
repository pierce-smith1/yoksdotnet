using System.ComponentModel;
using yoksdotnet.drawing;

using RgbColor = yoksdotnet.drawing.RgbColor;

namespace yoksdotnet.windows;

public class PaletteView(Palette _backingPalette) : INotifyPropertyChanged
{
    public Palette BackingPalette => _backingPalette;

    public RgbColor Scales
    {
        get => _backingPalette.scales;
        set 
        {
            _backingPalette.scales = value;
            OnPropertyChanged(nameof(Scales));
            OnPropertyChanged(nameof(ScalesHex));
        }
    }

    public RgbColor ScalesHighlight
    {
        get => _backingPalette.scalesHighlight;
        set 
        {
            _backingPalette.scalesHighlight = value;
            OnPropertyChanged(nameof(ScalesHighlight));
            OnPropertyChanged(nameof(ScalesHighlightHex));
        }
    }

    public RgbColor ScalesShadow
    {
        get => _backingPalette.scalesShadow;
        set 
        {
            _backingPalette.scalesShadow = value;
            OnPropertyChanged(nameof(ScalesShadow));
            OnPropertyChanged(nameof(ScalesShadowHex));
        }
    }

    public RgbColor Horns
    {
        get => _backingPalette.horns;
        set 
        {
            _backingPalette.horns = value;
            OnPropertyChanged(nameof(Horns));
            OnPropertyChanged(nameof(HornsHex));
        }
    }

    public RgbColor Eyes
    {
        get => _backingPalette.eyes;
        set 
        {
            _backingPalette.eyes = value;
            OnPropertyChanged(nameof(Eyes));
            OnPropertyChanged(nameof(EyesHex));
        }
    }

    public RgbColor Whites
    {
        get => _backingPalette.whites;
        set
        {
            _backingPalette.whites = value;
            OnPropertyChanged(nameof(Whites));
            OnPropertyChanged(nameof(WhitesHex));
        }
    }

    public RgbColor HornsShadow
    {
        get => _backingPalette.hornsShadow;
        set
        {
            _backingPalette.hornsShadow = value;
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
    public string ScalesHex => ColorConversion.ToHex(Scales);
    public string ScalesHighlightHex => ColorConversion.ToHex(ScalesHighlight);
    public string ScalesShadowHex => ColorConversion.ToHex(ScalesShadow);
    public string HornsHex => ColorConversion.ToHex(Horns);
    public string EyesHex => ColorConversion.ToHex(Eyes);
    public string WhitesHex => ColorConversion.ToHex(Whites);
    public string HornsShadowHex => ColorConversion.ToHex(HornsShadow);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
