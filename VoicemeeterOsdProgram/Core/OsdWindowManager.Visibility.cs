using System;
using System.Windows;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using VoicemeeterOsdProgram.Factories;

namespace VoicemeeterOsdProgram.Core
{
    partial class OsdWindowManager
    {
        private static void UpdateOsdElementsVis()
        {
            m_wpfControl.AllowAutoUpdateSeparators = false;

            var children = m_wpfControl.MainContent.Children;
            foreach (StripControl strip in children)
            {
                bool isAnyVisibleBtn = false;
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        isAnyVisibleBtn = true;
                        break;
                    }
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        isAnyVisibleBtn = true;
                        break;
                    }
                }
                foreach (ButtonContainer btnCont in strip.AdditionalControlBtns.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        isAnyVisibleBtn = true;
                        break;
                    }
                }

                bool isVisibleChildren = (strip.FaderCont.Visibility == Visibility.Visible) || isAnyVisibleBtn;
                if (isVisibleChildren)
                {
                    strip.Visibility = Visibility.Visible;
                    UpdateConstantVisibleElements(strip);
                }
            }

            m_wpfControl.UpdateSeparators();
            m_wpfControl.AllowAutoUpdateSeparators = true;
        }

        private static void UpdateConstantVisibleElements(StripControl strip)
        {
            var options = Options.OptionsStorage.Osd;
            foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
            {
                if (!options.AlwaysShowBusBtns) break;

                btnCont.Visibility = Visibility.Visible;
            }
            foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
            {
                bool hasConstantVisibleBtns = options.AlwaysShowMonoBtn || options.AlwaysShowMuteBtn || options.AlwaysShowSoloBtn;
                if (!hasConstantVisibleBtns) break;

                var style = btnCont.Btn.Style;
                if (options.AlwaysShowMuteBtn && style.Equals(StripButtonFactory.GetMuteStyle(btnCont)))
                {
                    btnCont.Visibility = Visibility.Visible;
                }
                else if (options.AlwaysShowSoloBtn && style.Equals(StripButtonFactory.GetSoloStyle(btnCont)))
                {
                    btnCont.Visibility = Visibility.Visible;
                }
                else if (options.AlwaysShowMonoBtn && style.Equals(StripButtonFactory.GetMonoStyle(btnCont)))
                {
                    btnCont.Visibility = Visibility.Visible;
                }
                else if (options.AlwaysShowMonoBtn && style.Equals(StripButtonFactory.GetMonoWithReverseStyle(btnCont)))
                {
                    btnCont.Visibility = Visibility.Visible;
                }
            }
        }

        private static void ApplyVisibilityToOsdElements(Visibility vis)
        {
            m_wpfControl.AllowAutoUpdateSeparators = false;

            var children = m_wpfControl.MainContent.Children;
            foreach (StripControl strip in children)
            {
                strip.Visibility = vis;
                strip.FaderCont.Visibility = vis;
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    btnCont.Visibility = vis;
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    btnCont.Visibility = vis;
                }
                foreach (ButtonContainer btnCont in strip.AdditionalControlBtns.Children)
                {
                    btnCont.Visibility = vis;
                }
            }

            m_wpfControl.UpdateSeparators();
            m_wpfControl.AllowAutoUpdateSeparators = true;
        }
    }
}
