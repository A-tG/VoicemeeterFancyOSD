using System;
using System.Windows;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Core
{
    partial class OsdWindowManager
    {

        internal static void RandomizeElementsState()
        {
            ApplyVisibilityToOsdElements(Visibility.Visible);

            m_wpfControl.AllowAutoUpdateSeparators = false;
            var random = new Random();
            var children = m_wpfControl.MainContent.Children;
            foreach (StripControl strip in children)
            {
                if (random.Next(100) < 50)
                {
                    strip.Visibility = Visibility.Collapsed;
                    continue;
                }
                else
                {
                    strip.Visibility = Visibility.Visible;
                }
                if (random.Next(100) < 50)
                {
                    strip.FaderCont.Visibility = Visibility.Collapsed;
                }
                else
                {
                    strip.FaderCont.Visibility = Visibility.Visible;
                    strip.FaderCont.Fader.Value = random.Next(72) - 60;
                }
                foreach (ButtonContainer btnCont in strip.BusBtnsContainer.Children)
                {
                    if (random.Next(100) < 75)
                    {
                        btnCont.Btn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnCont.Btn.Visibility = Visibility.Visible;
                        if (random.Next(100) < 50)
                        {
                            btnCont.Btn.State++;
                        }
                    }
                }
                foreach (ButtonContainer btnCont in strip.ControlBtnsContainer.Children)
                {
                    if (random.Next(100) < 25)
                    {
                        btnCont.Btn.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btnCont.Btn.Visibility = Visibility.Visible;
                        if (random.Next(100) < 50)
                        {
                            btnCont.Btn.State++;
                        }
                    }
                }
                m_wpfControl.UpdateSeparators();
                m_wpfControl.AllowAutoUpdateSeparators = true;
            }
        }
    }
}
