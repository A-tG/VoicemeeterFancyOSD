using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for LimiterContainer.xaml
    /// </summary>
    public partial class LimiterContainer : ContentControl
    {
        private DoubleAnimation m_highlightAnim = new()
        {
            From = 1,
            To = 0.0,
            EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn },
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            FillBehavior = FillBehavior.Stop
        };

        public LimiterContainer()
        {
            InitializeComponent();
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Slider slider) return;

            slider.Value = 12;
        }

        public void Highlight()
        {
            if (!Options.OptionsStorage.Osd.AnimationsEnabled) return;

            HighlightWrap.BeginAnimation(Border.OpacityProperty, m_highlightAnim);
        }
    }
}
