using System.Windows;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings;

/// <summary>
/// Interaction logic for AlignmentSelects.xaml
/// </summary>
public partial class AlignmentSelects : UserControl
{
    public AlignmentSelects()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty HorAlignmentValProperty = DependencyProperty.Register(
        nameof(HorAlignmentVal), typeof(HorAlignment), typeof(AlignmentSelects));
    public HorAlignment HorAlignmentVal
    {
        get => (HorAlignment)GetValue(HorAlignmentValProperty);
        set => SetValue(HorAlignmentValProperty, value);
    }

    public static readonly DependencyProperty VertAlignmentValProperty = DependencyProperty.Register(
        nameof(VertAlignmentVal), typeof(VertAlignment), typeof(AlignmentSelects));
    public VertAlignment VertAlignmentVal
    {
        get => (VertAlignment)GetValue(VertAlignmentValProperty);
        set => SetValue(VertAlignmentValProperty, value);
    }
}
