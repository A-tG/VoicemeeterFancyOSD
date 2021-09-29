using System.Windows;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Factories
{
    partial class OsdContentFactory
    {
        private static void MakeStripFaderParam(StripControl strip, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetInputGain(i));
            InitFaderParam(strip, ref p);
            m_vmParams.Add(p);
        }

        private static void MakeBusFaderParam(StripControl strip, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetBusGain(i));
            InitFaderParam(strip, ref p);
            m_vmParams.Add(p);
        }

        private static void InitFaderParam(StripControl strip, ref VoicemeeterParameter p)
        {
            p.ValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                strip.FaderCont.Visibility = Visibility.Visible;
                strip.FaderCont.Fader.Value = e.newVal;
            };
        }

        private static void InitBtnParam(StripControl strip, ButtonContainer btnCtn, ref VoicemeeterParameter p)
        {
            p.ValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                btnCtn.Visibility = Visibility.Visible;
                btnCtn.Btn.State = (uint)e.newVal;
            };
        }

        private static void MakeInputMonoParam(StripControl strip, ButtonContainer btnCtn, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetInputMono(i));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }

        private static void MakeBusMonoParam(StripControl strip, ButtonContainer btnCtn, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetBusMono(i));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }

        private static void MakeInputMuteParam(StripControl strip, ButtonContainer btnCtn, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetInputMute(i));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }

        private static void MakeBusMuteParam(StripControl strip, ButtonContainer btnCtn, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetBusMute(i));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }

        private static void MakeInputSoloParam(StripControl strip, ButtonContainer btnCtn, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetInputSolo(i));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }
    }
}
