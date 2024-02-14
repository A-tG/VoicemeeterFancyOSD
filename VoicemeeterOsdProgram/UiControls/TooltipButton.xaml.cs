using AtgDev.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VoicemeeterOsdProgram.UiControls;

/// <summary>
/// Interaction logic for TooltipButton.xaml
/// </summary>
public partial class TooltipButton : UserControl
{
    private bool m_isOpenedByHover;
    private PeriodicTimerExt m_hoverTimer = new(TimeSpan.FromSeconds(0.4));

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen), typeof(bool), typeof(TooltipButton));

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set
        {
            if (!value)
            {
                m_isOpenedByHover = false;
                m_hoverTimer.Stop();
            }
            SetValue(IsOpenProperty, value);
        }
    }

    public TooltipButton()
    {
        MouseLeave += (_, _) => OnLeave();
        MouseEnter += (_, _) => OnEnter();
        MouseMove += OnMove;
        Unloaded += (_, _) => m_hoverTimer.Stop();
        InitializeComponent();
    }

    private void OnMove(object sender, MouseEventArgs e)
    {
        var pos = e.GetPosition(this);
        var x = pos.X;
        var y = pos.Y;
        bool isOut = (x < 0) || (y < 0) ||
            (x > ActualWidth) || (y > ActualHeight);
        if (isOut)
        {
            OnLeave();
        }
    }

    private void OnLeave()
    {
        if (m_isOpenedByHover)
        {
            IsOpen = false;
        }
    }

    private async void OnEnter()
    {
        m_hoverTimer.Start();
        if (IsOpen) return;

        if (await m_hoverTimer.WaitForNextTickAsync())
        {
            if (IsOpen) return;

            m_isOpenedByHover = true;
            IsOpen = true;
        }
    }
}
