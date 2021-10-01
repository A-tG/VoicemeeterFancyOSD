using System.Windows;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using static VoicemeeterOsdProgram.Factories.VoicemeeterCommandsFactory;

namespace VoicemeeterOsdProgram.Factories
{
    partial class OsdContentFactory
    {
        private static void InitFaderParam(StripControl strip, ref VoicemeeterParameter p)
        {
            p.ReadValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                strip.FaderCont.Visibility = Visibility.Visible;
                strip.FaderCont.Fader.Value = e.newVal;
            };
        }

        private static void MakeFaderParam(StripControl strip, int i, StripType type)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, Gain(i, type));
            InitFaderParam(strip, ref p);
            m_vmParams.Add(p);
        }

        private static void InitBtnParam(StripControl strip, ButtonContainer btnCtn, ref VoicemeeterParameter p)
        {
            p.ReadValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                btnCtn.Visibility = Visibility.Visible;
                btnCtn.Btn.State = (uint)e.newVal;
            };
        }

        private static void MakeMonoParam(StripControl strip, ButtonContainer btnCtn, int i, StripType type)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, Mono(i, type));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }

        private static void MakeMuteParam(StripControl strip, ButtonContainer btnCtn, int i, StripType type)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, Mute(i, type));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }

        private static void MakeSoloParam(StripControl strip, ButtonContainer btnCtn, int i, StripType type)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, Solo(i, type));
            InitBtnParam(strip, btnCtn, ref p);
            m_vmParams.Add(p);
        }
    }
}
