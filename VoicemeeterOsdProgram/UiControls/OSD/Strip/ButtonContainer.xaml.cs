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
    public partial class ButtonContainer : ContentControl, IOsdAnimatedElement
    {
        private Duration m_animDuration = new(TimeSpan.FromMilliseconds(350));
        private DoubleAnimation m_highlightAnim = new()
        {
            From = 0.0,
            To = 1.15,
            EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop
        };
        private DoubleAnimation m_opacityAnim = new()
        {
            From = 0.5,
            To = 0.9,
            FillBehavior = FillBehavior.Stop
        };

        public ButtonContainer()
        {
            m_highlightAnim.Duration = m_animDuration;
            m_opacityAnim.Duration = m_animDuration;

            InitializeComponent();
        }

        public Func<bool> IsAnimationsEnabled { get; set; } = () => true;

        public void Highlight()
        {
            if (!IsAnimationsEnabled()) return;

            if (HighlightWrap.RenderTransform is not ScaleTransform t)
            {
                HighlightWrap.RenderTransform = t = new ScaleTransform(0, 0, 0.5d, 0.5d);
            }
            t.BeginAnimation(ScaleTransform.ScaleYProperty, m_highlightAnim);
            t.BeginAnimation(ScaleTransform.ScaleXProperty, m_highlightAnim);
            HighlightWrap.BeginAnimation(Border.OpacityProperty, m_opacityAnim);
        }
    }
}
