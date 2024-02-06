using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip;

/// <summary>
/// Interaction logic for StripControl.xaml
/// </summary>
public partial class StripControl : UserControl, IOsdRootElement, IOsdAnimatedElement
{
    private bool m_hasChanges = false;
    private bool m_hasChildVis = false;

    private DoubleAnimation m_highlightAnim = new()
    {
        From = 0.9,
        To = 0,
        Duration = TimeSpan.FromMilliseconds(600),
        EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn },
        FillBehavior = FillBehavior.HoldEnd
    };

    public Func<bool> IsAnimationsEnabled { get; set; } = () => true;

    /// <summary>
    /// Resets itself when read
    /// </summary>
    public bool HasChangesFlag
    {
        get
        {
            if (m_hasChanges)
            {
                m_hasChanges = false;
                return true;
            }
            return false;
        }
        set => m_hasChanges = value;
    }

    /// <summary>
    /// Resets itself when read
    /// </summary>
    public bool HasAnyChildVisibleFlag
    {
        get
        {
            if (m_hasChildVis)
            {
                m_hasChildVis = false;
                return true;
            }
            return false;
        }
        set => m_hasChildVis = value;
    }

    public StripControl()
    {
        InitializeComponent();
        IsVisibleChanged += OnIsVisibleChanged;
    }

    public static readonly DependencyProperty ShowHorizontalSeparatorAfterProperty = DependencyProperty.Register(
        "ShowHorizontalSeparatorAfter", typeof(bool), typeof(StripControl));
    public bool ShowHorizontalSeparatorAfter
    {
        get => (bool)GetValue(ShowHorizontalSeparatorAfterProperty);
        set
        {
            var thickness = BorderThickness;
            thickness.Right = value ? 1 : 0;
            BorderThickness = thickness;
            SetValue(ShowHorizontalSeparatorAfterProperty, value);
        }
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        bool isVisible = Visibility == Visibility.Visible;
        var wasVisible = (bool)e.OldValue;
        if (!(isVisible && !wasVisible)) return;

        Highlight();
    }

    private void Highlight()
    {
        if (!IsAnimationsEnabled()) return;

        HighlightWrap.BeginAnimation(Border.OpacityProperty, m_highlightAnim);
    }
}
