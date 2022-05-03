using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using static VoicemeeterOsdProgram.Factories.VoicemeeterCommandsFactory;

namespace VoicemeeterOsdProgram.Factories
{
    partial class OsdContentFactory
    {
        private enum BtnType
        {
            Mute,
            Mono,
            Solo,
            A,
            B,
            Sel,
            EqOn
        }

        private static void MakeLabelParam(StripControl strip, int index, string defaultLabel, StripType type)
        {
            VoicemeeterStrParam p = new(VoicemeeterApiClient.Api, Label(index, type));
            strip.defaultLabel = defaultLabel;
            strip.VmParameter = p;
            m_vmParams.Add(p);
        }

        private static void MakeFaderParam(StripControl strip, int i, StripType type)
        {
            var p = new VoicemeeterNumParam(VoicemeeterApiClient.Api, Gain(i, type));
            strip.FaderCont.VmParameter = p;
            m_vmParams.Add(p);
        }

        private static void MakeLimiterParam(StripControl strip, int i)
        {
            var p = new VoicemeeterNumParam(VoicemeeterApiClient.Api, Limiter(i));
            strip.LimiterCont.VmParameter = p;
            m_vmParams.Add(p);
        }

        private static VoicemeeterNumParam GetSelParam(int i, ButtonContainer btnCtn)
        {
            VoicemeeterNumParam p = new(VoicemeeterApiClient.Api, Sel(i));
            var selBtns = m_selButtons;
            selBtns.Add(btnCtn);

            // dirty hack to reset all Sel buttons to 0 before main click event, otherwise multiple Sel buttons will be active
            btnCtn.Btn.Click += (sender, _) =>
            {
                var btn = (OutlineTglBtn)sender;
                var oldVal = btn.State;
                foreach (var btnContainer in selBtns)
                {
                    btnContainer.Btn.State = 0;
                    btnContainer.VmParameter?.Write(0);
                }
                btn.State = ++oldVal;
            };

            return p;
        }

        private static void MakeButtonParam(BtnType bType, StripType sType, ButtonContainer btnCtn, int i, int busIndex = 0)
        {
            var api = VoicemeeterApiClient.Api;
            if (api is null) return;

            VoicemeeterNumParam p = bType switch
            {
                BtnType.Mono => new(api, Mono(i, sType)),
                BtnType.Mute => new(api, Mute(i, sType)),
                BtnType.Solo => new(api, Solo(i, sType)),
                BtnType.A => new(api, HardBusAssign(i, busIndex)),
                BtnType.B => new(api, VirtBusAssign(i, busIndex)),
                BtnType.Sel => GetSelParam(i, btnCtn),
                BtnType.EqOn => new(api, EqOn(i)),
                _ => null
            };
            if (p is null) return;

            btnCtn.VmParameter = p;
            m_vmParams.Add(p);
        }
    }
}
