using System;
using VoicemeeterOsdProgram.Core.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    partial class ButtonContainer
    {
        public Func<bool> IsAlwaysVisible = () => false;

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
                        Btn.Click -= OnBtnClick;
                        m_vmParam = null;
                    }
                    return;
                }

                m_vmParam = value;
                m_vmParam.ReadValueChanged += OnVmValueChanged;
                Btn.Click += OnBtnClick; ;
            }
        }

        private void OnVmValueChanged(object sender, ValOldNew<float> e)
        {
            if (OsdParent is not null)
            {
                OsdParent.HasChangesFlag = true;
            }

            Visibility = System.Windows.Visibility.Visible;
            Btn.State = (uint)e.newVal;
        }

        private void OnBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not OutlineTglBtn btn) return;

            m_vmParam.Write(btn.State);
        }
    }
}
