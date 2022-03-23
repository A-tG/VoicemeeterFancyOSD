using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    /// <summary>
    /// Interaction logic for ButtonContainer.xaml
    /// </summary>
    public partial class ButtonContainer : ContentControl
    {
        public IOsdRootElement OsdParent;
        private const int HighlightAnimFadeOutTimeMs = 150;

        private DoubleAnimation m_highlightAnim;

        public ButtonContainer()
        {
            InitializeComponent();

            m_highlightAnim = new()
            {
                From = 1.0,
                To = 0.0,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn },
                Duration = new Duration(TimeSpan.FromMilliseconds(HighlightAnimFadeOutTimeMs)),
                FillBehavior = FillBehavior.HoldEnd
            };
        }

        public void Highlight()
        {
            HighlightWrap.BeginAnimation(Border.OpacityProperty, m_highlightAnim);
        }
    }
}
