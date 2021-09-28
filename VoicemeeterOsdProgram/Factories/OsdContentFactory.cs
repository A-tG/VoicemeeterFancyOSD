using AtgDev.Voicemeeter.Types;
using System.Windows;
using System.Collections.Generic;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

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
                strip.StripLabel.Text = $"Hard In{i + 1}";

                vmParams.Add(GetStripParameters(strip, i));

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.virtInputs; i++)
            {
                var strip = GetVirtualInputStrip(properties);
                strip.StripLabel.Text = $"Virt In{i + 1}";

                vmParams.Add(GetStripParameters(strip, properties.hardInputs + i));

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.hardOutputs; i++)
            {
                var strip = GetOutputStrip();
                var name = properties.hardOutputs == 1 ? $"A" : $"A{i + 1}";
                strip.StripLabel.Text = name;

                vmParams.Add(GetBusParameters(strip,  i));

                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.virtOutputs; i++)
            {
                var strip = GetOutputStrip();
                var name = properties.virtOutputs == 1 ? $"B" : $"B{i + 1}";
                strip.StripLabel.Text = name;

                vmParams.Add(GetBusParameters(strip, properties.hardOutputs + i));

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

        private static VoicemeeterParameter GetStripParameters(StripControl strip, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetStripGain(i));
            p.ValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                strip.FaderCont.Visibility = Visibility.Visible;
                strip.FaderCont.Fader.Value = e.newVal;
            };
            return p;
        }

        private static VoicemeeterParameter GetBusParameters(StripControl strip, int i)
        {
            var p = new VoicemeeterParameter(VoicemeeterApiClient.Api, VoicemeeterCommandsFactory.GetBusGain(i));
            p.ValueChanged += (sender, e) =>
            {
                strip.Visibility = Visibility.Visible;
                strip.FaderCont.Visibility = Visibility.Visible;
                strip.FaderCont.Fader.Value = e.newVal;
            };
            return p;
        }
    }
}
