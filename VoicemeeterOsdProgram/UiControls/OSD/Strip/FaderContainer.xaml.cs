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
    public partial class FaderContainer : ContentControl
    {
        public IOsdRootElement OsdParent;

        private DoubleAnimation m_highlightAnim = new()
        {
            From = 0.9,
            To = 0.0,
            EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut },
            Duration = new Duration(TimeSpan.FromMilliseconds(250)),
            FillBehavior = FillBehavior.Stop
        };

        public FaderContainer()
        {
            InitializeComponent();
        }

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
            if (!Options.OptionsStorage.Osd.AnimationsEnabled) return;

            HighlightWrap.BeginAnimation(Border.OpacityProperty, m_highlightAnim);
        }
    }
}
