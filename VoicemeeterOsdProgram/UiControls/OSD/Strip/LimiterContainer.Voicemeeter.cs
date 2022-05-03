using System;
using System.Windows;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    partial class LimiterContainer : IOsdChildElement
    {
        public IOsdRootElement OsdParent { get; set; }

        public Func<bool> IsAlwaysVisible { get; set; } = () => false;

        public Func<bool> IsNeverShow { get; set; } = () => false;

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
                        Limiter.ValueChanged -= OnValueChanged;
                        m_vmParam = null;
                    }
                    return;
                }

                m_vmParam = value;
                m_vmParam.ReadValueChanged += OnVmValueChanged;
                Limiter.ValueChanged += OnValueChanged;
            }
        }

        private void OnVmValueChanged(object sender, ValOldNew<float> e)
        {
            bool hasOsdParent = OsdParent is not null;
            if (hasOsdParent)
            {
                OsdParent.HasChangesFlag = true;
            }

            if (!IsNeverShow())
            {
                Visibility = Visibility.Visible;
                Highlight();
                if (hasOsdParent)
                {
                    OsdParent.HasAnyChildVisibleFlag = true;
                }
            }

            // To prevent triggering OnFaderValueChanged
            Limiter.isCustomFlag = true;
            Limiter.Value = e.newVal;
            Limiter.isCustomFlag = false;
        }

        private void OnValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var limiter = sender as SliderExt;
            if ((limiter is null) || limiter.isCustomFlag) return;

            m_vmParam.Write((float)e.NewValue);
        }
    }
}
