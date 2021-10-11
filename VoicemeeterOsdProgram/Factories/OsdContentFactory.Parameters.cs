using System;
using System.Windows;
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
            B
        }

        private static void MakeStripLabelParam(StripControl strip, int index, string defaultLabel)
        {
            var api = VoicemeeterApiClient.Api;
            var stripLabel = strip.StripLabel;

            VoicemeeterStrParam p = new(api, InputLabel(index));
            p.ValueRead += (_, e) =>
            {
                string name = e.newVal;
                string newName = string.IsNullOrEmpty(name) ? defaultLabel : name;
                if (stripLabel.Text == newName) return;

                stripLabel.Text = newName;
            };
            p.ReadIsNotifyChanges(true);
            m_vmParams.Add(p);
        }

        private static void InitFaderParam(StripControl strip, VoicemeeterNumParam p)
        {
            var faderCont = strip.FaderCont;
            var fader = faderCont.Fader;

            p.ReadValueChanged += (sender, e) =>
            {
                faderCont.Visibility = Visibility.Visible;
                fader.isIgnoreValueChanged = true;
                fader.Value = e.newVal;
                fader.isIgnoreValueChanged = false;
            };
            fader.ValueChanged += (sender, e) =>
            {
                var fader = sender as ClrChangeSlider;
                if ((fader is null) || fader.isIgnoreValueChanged) return;

                p.Write((float)e.NewValue);
            };
        }

        private static void MakeFaderParam(StripControl strip, int i, StripType type)
        {
            var p = new VoicemeeterNumParam(VoicemeeterApiClient.Api, Gain(i, type));
            InitFaderParam(strip, p);
            m_vmParams.Add(p);
        }

        private static void InitBtnParam(ButtonContainer btnCtn, VoicemeeterNumParam p)
        {
            var btn = btnCtn.Btn;
            p.ReadValueChanged += (sender, e) =>
            {
                btnCtn.Visibility = Visibility.Visible;
                btn.State = (uint)e.newVal;
            };
            btn.Click += (sender, e) =>
            {
                if (sender is not OutlineTglBtn btn) return;

                p.Write(btn.State);
            };
        }

        private static void MakeButtonParam(BtnType bType, StripType sType, ButtonContainer btnCtn, int i, int busIndex = 0)
        {
            var api = VoicemeeterApiClient.Api;
            if (api is null) return;

            VoicemeeterNumParam p = bType switch
            {
                BtnType.Mono => new (api, Mono(i, sType)),
                BtnType.Mute => new (api, Mute(i, sType)),
                BtnType.Solo => new (api, Solo(i, sType)),
                BtnType.A => new (api, HardBusAssign(i, busIndex)),
                BtnType.B => new (api, VirtBusAssign(i, busIndex)),
                _ => null
            };
            if (p is null) return;

            InitBtnParam(btnCtn, p);
            m_vmParams.Add(p);
        }
    }
}
