using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        private const int HighlightAnimFadeOutTimeMs = 125;

        private DoubleAnimation m_highlightAnim;

        public ButtonContainer()
        {
            InitializeComponent();

            m_highlightAnim = new()
            {
                From = 0.0,
                To = 1.2,
                Duration = new Duration(TimeSpan.FromMilliseconds(HighlightAnimFadeOutTimeMs)),
                FillBehavior = FillBehavior.Stop
            };
        }

        public void Highlight()
        {
            if (!Options.OptionsStorage.Osd.AnimationsEnabled) return;

            if (HighlightWrap.RenderTransform is not ScaleTransform t)
            {
                HighlightWrap.RenderTransform = t = new ScaleTransform(0, 0, 0.5d, 0.5d);
            }
            t.BeginAnimation(ScaleTransform.ScaleYProperty, m_highlightAnim);
            t.BeginAnimation(ScaleTransform.ScaleXProperty, m_highlightAnim);
            HighlightWrap.BeginAnimation(Border.OpacityProperty, m_highlightAnim);
        }
    }
}
