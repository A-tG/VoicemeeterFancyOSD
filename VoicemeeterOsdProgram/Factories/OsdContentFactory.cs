using AtgDev.Voicemeeter.Types;
using System.Collections.Generic;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Factories
{
    public static partial class OsdContentFactory
    {
        private static List<VoicemeeterParameter> m_vmParams;
        private static VoicemeeterProperties m_vmProperties;

        public static void FillOsdWindow(ref OsdControl osd, ref VoicemeeterParameter[] vmParams, VoicemeeterType type)
        {
            m_vmProperties = new VoicemeeterProperties(type);
            m_vmParams = new();

            osd.AllowAutoUpdateSeparators = false;

            // adding Hardware Inputs
            for (int i = 0; i < m_vmProperties.hardInputs; i++)
            {
                var strip = GetHardwareInputStrip(i);
                VoicemeeterApiClient.Api.GetParameter(VoicemeeterCommandsFactory.InputLabel(i), out string name);
                strip.StripLabel.Text = string.IsNullOrEmpty(name) ? $"HardIn{i + 1}" : name;

                MakeFaderParam(strip, i, StripType.Input);

                osd.MainContent.Children.Add(strip);
            }

            // adding Virtual Inputs
            for (int i = 0; i < m_vmProperties.virtInputs; i++)
            {
                var stripIndex = m_vmProperties.hardInputs + i;

                var strip = GetVirtualInputStrip(stripIndex);
                VoicemeeterApiClient.Api.GetParameter(VoicemeeterCommandsFactory.InputLabel(stripIndex), out string name);
                strip.StripLabel.Text = string.IsNullOrEmpty(name) ? $"VirtIn{i + 1}" : name;

                MakeFaderParam(strip, stripIndex, StripType.Input);

                osd.MainContent.Children.Add(strip);
            }

            // adding Physical Outputs
            for (int i = 0; i < m_vmProperties.hardOutputs; i++)
            {
                var strip = GetOutputStrip(i);
                var name = m_vmProperties.hardOutputs == 1 ? $"A" : $"A{i + 1}";
                strip.StripLabel.Text = name;

                MakeFaderParam(strip, i, StripType.Output);

                osd.MainContent.Children.Add(strip);
            }

            // adding Virtual Outputs
            for (int i = 0; i < m_vmProperties.virtOutputs; i++)
            {
                var stripIndex = m_vmProperties.hardOutputs + i;
                var strip = GetOutputStrip(stripIndex);
                var name = m_vmProperties.virtOutputs == 1 ? $"B" : $"B{i + 1}";
                strip.StripLabel.Text = name;

                MakeFaderParam(strip, m_vmProperties.hardOutputs + i, StripType.Output);

                osd.MainContent.Children.Add(strip);
            }

            osd.UpdateSeparators();
            osd.AllowAutoUpdateSeparators = true;

            // read first time to initialize values
            foreach (var p in m_vmParams)
            {
                p.Read();
            }

            vmParams = m_vmParams.ToArray();
            m_vmParams.Clear();
            m_vmParams = null;
        }

        private static StripControl GetOutputStrip(int stripIndex)
        {
            var strip = new StripControl();

            var btn = StripButtonFactory.GetMonoWithReverse();
            MakeMonoParam(strip, btn, stripIndex, StripType.Output);
            strip.ControlBtnsContainer.Children.Add(btn);

            btn = StripButtonFactory.GetMute();
            MakeMuteParam(strip, btn, stripIndex, StripType.Output);
            strip.ControlBtnsContainer.Children.Add(btn);
            return strip;
        }

        private static StripControl GetInput(int stripIndex)
        {
            var strip = new StripControl();

            var btn = StripButtonFactory.GetSolo();
            MakeSoloParam(strip, btn, stripIndex, StripType.Input);
            strip.ControlBtnsContainer.Children.Add(btn);

            btn = StripButtonFactory.GetMute();
            MakeMuteParam(strip, btn, stripIndex, StripType.Input);
            strip.ControlBtnsContainer.Children.Add(btn);

            for (int i = 0; i < m_vmProperties.hardOutputs; i++)
            {
                var btnCont = StripButtonFactory.GetBusSelect();
                var name = (m_vmProperties.hardOutputs == 1) ? $"A" : $"A{i + 1}";
                btnCont.Btn.Content = name;
                strip.BusBtnsContainer.Children.Add(btnCont);
            }

            for (int i = 0; i < m_vmProperties.virtOutputs; i++)
            {
                var btnCont = StripButtonFactory.GetBusSelect();
                var name = (m_vmProperties.virtOutputs == 1) ? $"B" : $"B{i + 1}";
                btnCont.Btn.Content = name;
                strip.BusBtnsContainer.Children.Add(btnCont);
            }

            return strip;
        }

        private static StripControl GetVirtualInputStrip(int stripIndex)
        {
            return GetInput(stripIndex);
        }

        private static StripControl GetHardwareInputStrip(int stripIndex)
        {
            var strip = GetInput(stripIndex);
            var btn = StripButtonFactory.GetMono();
            MakeMonoParam(strip, btn, stripIndex, StripType.Input);
            strip.ControlBtnsContainer.Children.Insert(0, btn);
            return strip;
        }
    }
}
