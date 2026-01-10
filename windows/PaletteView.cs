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
    public string ScalesHex => BackingPalette.ScalesHex;

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
    public string ScalesHighlightHex => BackingPalette.ScalesHighlightHex;

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
    public string ScalesShadowHex => BackingPalette.ScalesShadowHex;

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
    public string HornsHex => BackingPalette.HornsHex;


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
    public string EyesHex => BackingPalette.EyesHex;

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
    public string WhitesHex => BackingPalette.WhitesHex;

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
    public string HornsShadowHex => BackingPalette.HornsShadowHex;

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

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
