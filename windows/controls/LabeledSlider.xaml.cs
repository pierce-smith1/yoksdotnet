using System.Windows;
using System.Windows.Controls;

using static System.Windows.DependencyProperty;

namespace yoksdotnet.windows.controls;

public partial class LabeledSlider : UserControl
{
    // This shit is UTTERLY UNACCEPTABLE, but every alternative is worse.
    // Close your eyes, take a deep breath, and count to 10. Everything will be okay.

    public static readonly DependencyProperty ValueProperty = Register(nameof(Value), typeof(double), typeof(LabeledSlider));
    public double Value
    {
        get => (double) GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty LeftLabelProperty = Register(nameof(LeftLabel), typeof(string), typeof(LabeledSlider));
    public string LeftLabel
    {
        get => (string) GetValue(LeftLabelProperty);
        set => SetValue(LeftLabelProperty, value);
    }

    public static readonly DependencyProperty RightLabelProperty = Register(nameof(RightLabel), typeof(string), typeof(LabeledSlider));
    public string RightLabel
    {
        get => (string) GetValue(RightLabelProperty);
        set => SetValue(RightLabelProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = Register(nameof(Minimum), typeof(double), typeof(LabeledSlider));
    public double Minimum
    {
        get => (double) GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty = Register(nameof(Maximum), typeof(double), typeof(LabeledSlider));
    public double Maximum
    {
        get => (double) GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public LabeledSlider()
    {
        InitializeComponent();
    }
}
