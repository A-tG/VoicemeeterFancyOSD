using VoicemeeterOsdProgram.Core.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    partial class FaderContainer
    {
        private VoicemeeterNumParam m_vmParam;

        public VoicemeeterNumParam VmParameter
        {
            get => m_vmParam;
            set
            {
                if (value is null)
                {
                    if (m_vmParam is not null)
                    {
                        m_vmParam.ReadValueChanged -= OnVmValueChanged;
                        Fader.ValueChanged -= OnFaderValueChanged;
                        m_vmParam = null;
                    }
                    return;
                }

                m_vmParam = value;
                m_vmParam.ReadValueChanged += OnVmValueChanged;
                Fader.ValueChanged += OnFaderValueChanged;
            }
        }

        private void OnVmValueChanged(object sender, ValOldNew<float> e)
        {
            if (ParentStrip is not null)
            {
                ParentStrip.HasChangesFlag = true;
            }

            Visibility = System.Windows.Visibility.Visible;

            // To prevent triggering OnFaderValueChanged
            Fader.isIgnoreValueChanged = true;
            Fader.Value = e.newVal;
            Fader.isIgnoreValueChanged = false;
        }

        private void OnFaderValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var fader = sender as ClrChangeSlider;
            if ((fader is null) || fader.isIgnoreValueChanged) return;

            m_vmParam.Write((float)e.NewValue);
        }
    }
}
