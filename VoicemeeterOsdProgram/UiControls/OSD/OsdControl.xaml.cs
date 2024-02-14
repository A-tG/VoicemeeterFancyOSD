using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.UiControls.OSD;

/// <summary>
/// Interaction logic for OSD.xaml
/// </summary>
public partial class OsdControl : UserControl
{
    public OsdControl()
    {
        InitializeComponent();
        MainContent.SizeChanged += OnSizeChange;
        LayoutUpdated += OnLayoutUpdate;
        
    }

    public bool AllowAutoUpdateSeparators { get; set; } = true;

    private double m_scale = 1;
    private bool m_isInteractable;
    private double m_maxW;
    private double m_maxH;

    public double MaxW
    {
        get => m_maxW;
        set
        {
            var w = value / Scale;
            MainContentWrap.MaxWidth = w;
            MainContent.MaxWidth = w;
            m_maxW = value;
        }
    }

    public double MaxH
    {
        get => m_maxH;
        set
        {
            MainContentWrap.MaxHeight = value / Scale;
            m_maxH = value;
        }
    }

    public double Scale
    {
        get => m_scale;
        set
        {
            m_scale = value;
            LayoutTransform = new ScaleTransform(value, value);
            MaxW = m_maxW;
            MaxH = m_maxH;
        }
    }

    public bool IsInteractable
    {
        get => m_isInteractable;
        set
        {
            if (m_isInteractable == value) return;

            CloseBtn.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            m_isInteractable = value;
        }
    }

    public void UpdateSeparators()
    {
        WrapPanelExt cont = MainContent;
        var lines = cont.GetChildrenLines();
        foreach (var line in lines)
        {
            List<UIElement> visibleElements = new();
            foreach (var el in line)
            {
                var vis = el.Visibility;
                if ((vis == Visibility.Visible) || (vis == Visibility.Hidden) || el.IsVisible)
                {
                    visibleElements.Add(el);
                }
            }
            var len = visibleElements.Count;
            if (len == 0) continue;
            foreach (StripControl item in visibleElements)
            {
                item.ShowHorizontalSeparatorAfter = true;
            }
            var lastInLine = (StripControl)visibleElements[^1];
            lastInLine.ShowHorizontalSeparatorAfter = false;
        }
    }

    // Copied ViewboxExtensions.GetChildScaleX() from WinRTXamlToolkit project
    public double GetContentScaleX()
    {
        Viewbox viewbox = MainContentWrap;
        if (viewbox.Child == null) return 1;

        var fe = viewbox.Child as FrameworkElement;

        if ((fe is null) || (fe.ActualWidth == 0)) return 1;

        return viewbox.ActualWidth / fe.ActualWidth;
    }

    public double GetContentScaleY()
    {
        Viewbox viewbox = MainContentWrap;
        if (viewbox.Child == null) return 1;

        var fe = viewbox.Child as FrameworkElement;

        if ((fe is null) || (fe.ActualHeight == 0)) return 1;

        return viewbox.ActualHeight / fe.ActualHeight;
    }

    private void OnSizeChange(object sender, SizeChangedEventArgs e)
    {
        if (AllowAutoUpdateSeparators)
        {
            UpdateSeparators();
        }
    }

    private void OnLayoutUpdate(object sender, System.EventArgs e)
    {
        var scale = GetContentScaleX();
        if (scale < 1)
        {
            Scale *= System.Math.Sqrt(scale);
        }
    }
}
