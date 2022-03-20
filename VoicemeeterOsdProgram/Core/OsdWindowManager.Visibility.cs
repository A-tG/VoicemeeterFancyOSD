﻿using System.Windows;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Core
{
    partial class OsdWindowManager
    {
        private static bool UpdateOsdElementsVis()
        {
            m_wpfControl.AllowAutoUpdateSeparators = false;

            bool hasAnyChildVisible = false;
            var children = m_wpfControl.MainContent.Children;
            foreach (StripControl strip in children)
            {
                // Element become visible if it's Voicemeeter Parameter is changed
                bool hasVisibleBtn = false;
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        hasVisibleBtn = true;
                        break;
                    }
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        hasVisibleBtn = true;
                        break;
                    }
                }
                foreach (ButtonContainer btnCont in strip.AdditionalControlBtns.Children)
                {
                    if (btnCont.Visibility == Visibility.Visible)
                    {
                        hasVisibleBtn = true;
                        break;
                    }
                }

                bool hasAnyVisibleElements = (strip.FaderCont.Visibility == Visibility.Visible) || hasVisibleBtn;
                if (hasAnyVisibleElements)
                {
                    strip.Visibility = Visibility.Visible;
                    UpdateAlwaysVisibleElements(strip);
                    hasAnyChildVisible = true;
                }
            }

            m_wpfControl.UpdateSeparators();
            m_wpfControl.AllowAutoUpdateSeparators = true;

            return hasAnyChildVisible;
        }

        private static void UpdateAlwaysVisibleElements(StripControl strip)
        {
            var options = Options.OptionsStorage.Osd;
            foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
            {
                if (!options.AlwaysShowElements.Contains(StripElements.Buses)) break;

                btnCont.Visibility = Visibility.Visible;
            }

            foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
            {
                if (btnCont.IsAlwaysVisible?.Invoke() ?? false)
                {
                    btnCont.Visibility = Visibility.Visible;
                }
            }

            if (options.AlwaysShowElements.Contains(StripElements.Fader))
            {
                strip.FaderCont.Visibility = Visibility.Visible;
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
