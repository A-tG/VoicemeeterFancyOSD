using AtgDev.Voicemeeter.Types;
using VoicemeeterOsdProgram.Core.Types;
using VoicemeeterOsdProgram.UiControls.OSD;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;

namespace VoicemeeterOsdProgram.Factories
{
    public static class OsdContentFactory
    {
        public static void FillOsdWindow(ref OsdControl osd, VoicemeeterType type)
        {
            var properties = new VoicemeeterProperties(type);
            osd.MainContent.Children.Clear();

            osd.AllowAutoUpdateSeparators = false;
            StripControl strip;

            for (int i = 0; i < properties.hardInputs; i++)
            {
                strip = GetHardwareInputStrip(properties);
                strip.StripLabel.Text = $"Hard In{i + 1}";
               
                osd.MainContent.Children.Add(strip);
            }
            for (int i = 0; i < properties.virtInputs; i++)
            {
                strip = GetVirtualInputStrip(properties);
                strip.StripLabel.Text = $"Virt In{i + 1}";
                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.hardOutputs; i++)
            {
                strip = GetOutputStrip();
                var name = properties.hardOutputs == 1 ? $"A" : $"A{i + 1}";
                strip.StripLabel.Text = name;
                osd.MainContent.Children.Add(strip);
            }

            for (int i = 0; i < properties.virtOutputs; i++)
            {
                strip = GetOutputStrip();
                var name = properties.virtOutputs == 1 ? $"B" : $"B{i + 1}";
                strip.StripLabel.Text = name;
                osd.MainContent.Children.Add(strip);
            }

            osd.UpdateSeparators();
            osd.AllowAutoUpdateSeparators = true;
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

        public static StripControl GetHardwareInputStrip(VoicemeeterProperties properties)
        {
            var strip = GetInput(properties);
            strip.ControlBtnsContainer.Children.Insert(0, StripButtonFactory.GetMono());
            return strip;
        }
    }
}
