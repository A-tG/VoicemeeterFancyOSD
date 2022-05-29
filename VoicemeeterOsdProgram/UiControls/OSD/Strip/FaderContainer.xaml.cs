using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for FaderContainer.xaml
    /// </summary>
    public partial class FaderContainer : ContentControl, IOsdAnimatedElement
    {
        private DoubleAnimation m_highlightAnim = new()
        {
            From = 1,
            To = 0.0,
            EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn },
            Duration = new Duration(TimeSpan.FromMilliseconds(500)),
            FillBehavior = FillBehavior.Stop
        };

        public FaderContainer()
        {
            InitializeComponent();
        }

        public Func<bool> IsAnimationsEnabled { get; set; } = () => true;

        private void OnFaderMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not Slider slider) return;

            double val = 3;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                val = 1;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                val = 0.1;
            }
            if (e.Delta < 0) val *= -1;

            slider.Value += val;
        }

        private void OnFaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Slider slider) return;

            slider.Value = 0;
        }

        public void Highlight()
        {
            if (!IsAnimationsEnabled()) return;

            HighlightWrap.BeginAnimation(Border.OpacityProperty, m_highlightAnim);
        }
    }
}
