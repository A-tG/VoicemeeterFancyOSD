using System.Windows;
using VoicemeeterOsdProgram.Options;
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
            var len = children.Count;
            for (int i = 0; i < len; i++)
            {
                var strip = (StripControl)children[i];
                // 2 checks to imitate "lazy" evaluation
                bool hasChanges = strip.HasChangesFlag;
                if (!hasChanges) continue;

                bool isIgnore = OptionsStorage.Osd.IgnoreStripsIndexes.Contains((uint)i);
                if (isIgnore) continue;

                strip.Visibility = Visibility.Visible;
                UpdateAlwaysVisibleElements(strip);
                hasAnyChildVisible = true;
            }

            m_wpfControl.UpdateSeparators();
            m_wpfControl.AllowAutoUpdateSeparators = true;

            return hasAnyChildVisible;
        }

        private static void UpdateAlwaysVisibleElements(StripControl strip)
        {
            var options = OptionsStorage.Osd;
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
