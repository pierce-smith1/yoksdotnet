using System.Collections.Generic;
using System.Linq;
using System.Windows;
using yoksdotnet.common;
using yoksdotnet.logic.scene.patterns;
using yoksdotnet.windows.common;

namespace yoksdotnet.windows;

public partial class PatternAvailabilityCustomizer : Window
{
    public PatternAvailabilityCustomizer(List<Pattern> enabledPatterns)
    {
        InitializeComponent();

        var entries = SfEnums.GetAll<Pattern>()
            .Select(p => new PatternAvailabilityEntry(ViewModel, p, enabledPatterns.Contains(p)))
            .ToList();

        ViewModel.Entries = entries;
    }

    private void OnCancel(object _sender, RoutedEventArgs _e)
    {
        DialogResult = false;
    }

    private void OnAccept(object _sender, RoutedEventArgs _e)
    {
        DialogResult = true;
    }
}

public class PatternAvailabilityViewModel : NotifiesPropertyChanged
{
    private List<PatternAvailabilityEntry> _entries = [];
    public List<PatternAvailabilityEntry> Entries
    {
        get => _entries;
        set 
        {
            _entries = value;
            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(CanSave));
        }
    }

    public bool CanSave => Entries.Any(e => e.Enabled);
}

public class PatternAvailabilityEntry(PatternAvailabilityViewModel parentModel, Pattern pattern, bool enabled) : NotifiesPropertyChanged
{
    public Pattern Pattern { get; set; } = pattern;

    private bool _enabled = enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            OnPropertyChanged(nameof(Enabled));
            OnPropertyChanged(nameof(TextStyle));
            OnPropertyChanged(nameof(TextWeight));

            parentModel.OnPropertyChanged(nameof(parentModel.CanSave));
        }
    }

    public string TextStyle => Enabled ? "Normal" : "Italic";
    public string TextWeight => Enabled ? "Bold" : "Normal";
}
