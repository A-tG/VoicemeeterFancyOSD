using VoicemeeterOsdProgram.Core.Types;
using System.Windows;
using VoicemeeterOsdProgram.Types;
using System;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip
{
    partial class FaderContainer : IOsdChildElement
    {
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
