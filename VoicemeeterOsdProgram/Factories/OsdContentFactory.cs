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

        public static void FillOsdWindow(ref OsdControl osd, ref VoicemeeterParameter[] vmParams, VoicemeeterType type)
        {
            var properties = new VoicemeeterProperties(type);
            m_vmParams = new();

            osd.AllowAutoUpdateSeparators = false;

            for (int i = 0; i < properties.hardInputs; i++)
            {
                var strip = GetHardwareInputStrip(properties, i);
                VoicemeeterApiClient.Api.GetParameter(VoicemeeterCommandsFactory.GetInputLabel(i), out string name);
                strip.StripLabel.Text = string.IsNullOrEmpty(name) ? $"Hard In{i + 1}" : name;

                MakeStripFaderParam(strip, i);

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.virtInputs; i++)
            {
                var stripIndex = properties.hardInputs + i;
                var strip = GetVirtualInputStrip(properties, stripIndex);
                VoicemeeterApiClient.Api.GetParameter(VoicemeeterCommandsFactory.GetInputLabel(stripIndex), out string name);
                strip.StripLabel.Text = string.IsNullOrEmpty(name) ? $"Virt In{i + 1}" : name;

                MakeStripFaderParam(strip, stripIndex);

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.hardOutputs; i++)
            {
                var strip = GetOutputStrip(i);
                var name = properties.hardOutputs == 1 ? $"A" : $"A{i + 1}";
                strip.StripLabel.Text = name;

                MakeBusFaderParam(strip,  i);

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.virtOutputs; i++)
            {
                var stripIndex = properties.hardOutputs + i;
                var strip = GetOutputStrip(stripIndex);
                var name = properties.virtOutputs == 1 ? $"B" : $"B{i + 1}";
                strip.StripLabel.Text = name;

                MakeBusFaderParam(strip, properties.hardOutputs + i);

                osd.MainContent.Children.Add(strip);
            }

            osd.UpdateSeparators();
            osd.AllowAutoUpdateSeparators = true;

            foreach (var p in m_vmParams)
            {
                p.Read();
            }

            vmParams = m_vmParams.ToArray();
        }

        private static StripControl GetOutputStrip(int stripIndex)
        {
            var strip = new StripControl();

            var btn = StripButtonFactory.GetMonoWithReverse();
            MakeBusMonoParam(strip, btn, stripIndex);
            strip.ControlBtnsContainer.Children.Add(btn);

            btn = StripButtonFactory.GetMute();
            MakeBusMuteParam(strip, btn, stripIndex);
            strip.ControlBtnsContainer.Children.Add(btn);
            return strip;
        }

        private static StripControl GetInput(VoicemeeterProperties properties, int stripIndex)
        {
            var strip = new StripControl();
            var btn = StripButtonFactory.GetSolo();
            MakeInputSoloParam(strip, btn, stripIndex);
            strip.ControlBtnsContainer.Children.Add(btn);

            btn = StripButtonFactory.GetMute();
            MakeInputMuteParam(strip, btn, stripIndex);
            strip.ControlBtnsContainer.Children.Add(btn);

            for (int i = 0; i < properties.hardOutputs; i++)
            {
                var btnCont = StripButtonFactory.GetBusSelect();
                var name = (properties.hardOutputs == 1) ? $"A" : $"A{i + 1}";
                btnCont.Btn.Content = name;
                strip.BusBtnsContainer.Children.Add(btnCont);
            }

            for (int i = 0; i < properties.virtOutputs; i++)
            {
                var btnCont = StripButtonFactory.GetBusSelect();
                var name = (properties.virtOutputs == 1) ? $"B" : $"B{i + 1}";
                btnCont.Btn.Content = name;
                strip.BusBtnsContainer.Children.Add(btnCont);
            }

            return strip;
        }

        private static StripControl GetVirtualInputStrip(VoicemeeterProperties properties, int stripIndex)
        {
            return GetInput(properties, stripIndex);
        }

        private static StripControl GetHardwareInputStrip(VoicemeeterProperties properties, int stripIndex)
        {
            var strip = GetInput(properties, stripIndex);
            var btn = StripButtonFactory.GetMono();
            MakeInputMonoParam(strip, btn, stripIndex);
            strip.ControlBtnsContainer.Children.Insert(0, btn);
            return strip;
        }
    }
}
