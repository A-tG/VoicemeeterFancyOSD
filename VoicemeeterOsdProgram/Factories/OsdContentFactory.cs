using AtgDev.Voicemeeter.Types;
using System.Windows;
using System.Collections.Generic;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using System;

namespace VoicemeeterOsdProgram.Factories
{
    public static class OsdContentFactory
    {
        public static void FillOsdWindow(ref OsdControl osd, ref VoicemeeterParameter[] parameters, VoicemeeterType type)
        {
            var properties = new VoicemeeterProperties(type);

            osd.AllowAutoUpdateSeparators = false;
            List<VoicemeeterParameter> vmParams = new();

            for (int i = 0; i < properties.hardInputs; i++)
            {
                var strip = GetHardwareInputStrip(properties);
                VoicemeeterApiClient.Api.GetParameter(VoicemeeterCommandsFactory.GetStripLabel(i), out string name);
                strip.StripLabel.Text = (String.IsNullOrEmpty(name)) ? $"Hard In{i + 1}" : name;

                GetStripParameters(vmParams, strip, i);

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.virtInputs; i++)
            {
                var stripIndex = properties.hardInputs + i;
                var strip = GetVirtualInputStrip(properties);
                VoicemeeterApiClient.Api.GetParameter(VoicemeeterCommandsFactory.GetStripLabel(stripIndex), out string name);
                strip.StripLabel.Text = (String.IsNullOrEmpty(name)) ? $"Virt In{i + 1}" : name;

                GetStripParameters(vmParams, strip, stripIndex);

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.hardOutputs; i++)
            {
                var strip = GetOutputStrip();
                var name = properties.hardOutputs == 1 ? $"A" : $"A{i + 1}";
                strip.StripLabel.Text = name;

                GetBusParameters(vmParams, strip,  i);

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.virtOutputs; i++)
            {
                var strip = GetOutputStrip();
                var name = properties.virtOutputs == 1 ? $"B" : $"B{i + 1}";
                strip.StripLabel.Text = name;

                GetBusParameters(vmParams, strip, properties.hardOutputs + i);

                osd.MainContent.Children.Add(strip);
            }

            osd.UpdateSeparators();
            osd.AllowAutoUpdateSeparators = true;

            foreach (var p in vmParams)
            {
                p.Read();
            }

            parameters = vmParams.ToArray();
        }

        private static StripControl GetOutputStrip()
        {
            var strip = new StripControl();
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetMonoWithReverse());
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetMute());
            return strip;
        }

        private static StripControl GetInput(VoicemeeterProperties properties)
        {
            var strip = new StripControl();
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetSolo());
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetMute());

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

        private static StripControl GetVirtualInputStrip(VoicemeeterProperties properties)
        {
            return GetInput(properties);
        }

        private static StripControl GetHardwareInputStrip(VoicemeeterProperties properties)
        {
            var strip = GetInput(properties);
            strip.ControlBtnsContainer.Children.Insert(0, StripButtonFactory.GetMono());
            return strip;
        }

        private static void GetStripParameters(List<VoicemeeterParameter> parameters, StripControl strip, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetStripGain(i));
            p.ValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                strip.FaderCont.Visibility = Visibility.Visible;
                strip.FaderCont.Fader.Value = e.newVal;
            };
            parameters.Add(p);
        }

        private static void GetBusParameters(List<VoicemeeterParameter> parameters, StripControl strip, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetBusGain(i));
            p.ValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                strip.FaderCont.Visibility = Visibility.Visible;
                strip.FaderCont.Fader.Value = e.newVal;
            };
            parameters.Add(p);
        }
    }
}
