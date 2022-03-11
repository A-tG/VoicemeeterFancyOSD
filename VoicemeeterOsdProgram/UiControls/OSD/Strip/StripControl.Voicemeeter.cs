using VoicemeeterOsdProgram.Core.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    partial class StripControl
    {
        public string defaultLabel = string.Empty;

        private VoicemeeterStrParam m_vmParam;

        public ButtonContainer MuteButton { get; set; }

        public VoicemeeterStrParam VmParameter
        {
            get => m_vmParam;
            set
            {
                if (value is null)
                {
                    if (m_vmParam is not null)
                    {
                        m_vmParam.ValueRead -= OnVmValueRead;
                        m_vmParam = null;
                    }
                    return;
                }

                m_vmParam = value;
                m_vmParam.ValueRead += OnVmValueRead;
                m_vmParam.ReadNotifyChanges();
            }
        }

        public VoicemeeterNumParam VmFaderParameter
        {
            get => FaderCont.VmParameter;
            set
            {
                if (value is null)
                {
                    if (FaderCont.VmParameter is not null)
                    {
                        FaderCont.VmParameter.ReadValueChanged -= OnVmGainChanged;
                        FaderCont.VmParameter = null;
                    }
                    return;
                }

                FaderCont.VmParameter = value;
                FaderCont.VmParameter.ReadValueChanged += OnVmGainChanged;
            }
        }

        private void OnVmValueRead(object sender, ValOldNew<string> e)
        {
            string name = e.newVal;
            string newName = string.IsNullOrEmpty(name) ? defaultLabel : name;
            if (StripLabel.Text == newName) return;

            StripLabel.Text = newName;
        }

        private void OnVmGainChanged(object sender, ValOldNew<float> e)
        {
            if (MuteButton != null)
            {
                MuteButton.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
