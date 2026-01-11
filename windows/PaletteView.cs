using System.ComponentModel;
using yoksdotnet.drawing;

using Color = yoksdotnet.drawing.Color;

namespace yoksdotnet.windows;

public record PaletteView(Palette BackingPalette) : INotifyPropertyChanged
{
    public Color Scales
    {
        get => BackingPalette.Scales;
        set 
        {
            BackingPalette.Scales = value;
            OnPropertyChanged(nameof(Scales));
            OnPropertyChanged(nameof(ScalesHex));
        }
    }

    public Color ScalesHighlight
    {
        get => BackingPalette.ScalesHighlight;
        set 
        {
            BackingPalette.ScalesHighlight = value;
            OnPropertyChanged(nameof(ScalesHighlight));
            OnPropertyChanged(nameof(ScalesHighlightHex));
        }
    }

    public Color ScalesShadow
    {
        get => BackingPalette.ScalesShadow;
        set 
        {
            BackingPalette.ScalesShadow = value;
            OnPropertyChanged(nameof(ScalesShadow));
            OnPropertyChanged(nameof(ScalesShadowHex));
        }
    }

    public Color Horns
    {
        get => BackingPalette.Horns;
        set 
        {
            BackingPalette.Horns = value;
            OnPropertyChanged(nameof(Horns));
            OnPropertyChanged(nameof(HornsHex));
        }
    }

    public Color Eyes
    {
        get => BackingPalette.Eyes;
        set 
        {
            BackingPalette.Eyes = value;
            OnPropertyChanged(nameof(Eyes));
            OnPropertyChanged(nameof(EyesHex));
        }
    }

    public Color Whites
    {
        get => BackingPalette.Whites;
        set
        {
            BackingPalette.Whites = value;
            OnPropertyChanged(nameof(Whites));
            OnPropertyChanged(nameof(WhitesHex));
        }
    }

    public Color HornsShadow
    {
        get => BackingPalette.HornsShadow;
        set
        {
            BackingPalette.HornsShadow = value;
            OnPropertyChanged(nameof(HornsShadow));
            OnPropertyChanged(nameof(HornsShadowHex));
        }
    }

    public Color this[PaletteIndex index]
    {
        get => BackingPalette[index];
        set
        {
            BackingPalette[index] = value;
            OnPropertyChanged(index.Name);
            OnPropertyChanged($"{index.Name}Hex");
        }
    }
    public string ScalesHex => Scales.AsHex();
    public string ScalesHighlightHex => ScalesHighlight.AsHex();
    public string ScalesShadowHex => ScalesShadow.AsHex();
    public string HornsHex => Horns.AsHex();
    public string EyesHex => Eyes.AsHex();
    public string WhitesHex => Whites.AsHex();
    public string HornsShadowHex => HornsShadow.AsHex();

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
