﻿using System;
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

        private static void InitFaderParam(StripControl strip, ref VoicemeeterParameter p)
        {
            p.ReadValueChanged += (sender, e) =>
            {
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

        private static void InitBtnParam(ButtonContainer btnCtn, ref VoicemeeterParameter p)
        {
            p.ReadValueChanged += (sender, e) =>
            {
                btnCtn.Visibility = Visibility.Visible;
                btnCtn.Btn.State = (uint)e.newVal;
            };
        }

        private static void MakeButtonParam(BtnType bType, StripType sType, ButtonContainer btnCtn, int i, int busIndex = 0)
        {
            var api = VoicemeeterApiClient.Api;
            VoicemeeterParameter p = bType switch
            {
                BtnType.Mono => new (api, Mono(i, sType)),
                BtnType.Mute => new (api, Mute(i, sType)),
                BtnType.Solo => new (api, Solo(i, sType)),
                BtnType.A => new (api, HardBusAssign(i, busIndex)),
                BtnType.B => new (api, VirtBusAssign(i, busIndex)),
                _ => new (null, string.Empty)
            }; 
            InitBtnParam(btnCtn, ref p);
            m_vmParams.Add(p);
        }
    }
}
