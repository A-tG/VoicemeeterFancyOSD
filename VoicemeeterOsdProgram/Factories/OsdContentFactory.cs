using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoicemeeterOsdProgram.UiControls.OSD;
using AtgDev.Voicemeeter.Types;
using VoicemeeterOsdProgram.UiControls.OSD.Strip;
using System.Windows;

namespace VoicemeeterOsdProgram.Factories
{
    public static class OsdContentFactory
    {
        public static void FillOsdWindow(ref OsdControl osd, VoicemeeterType type)
        {
            int hardInputs = 0;
            int virtInputs = 0;
            int hardOutputs = 0;
            int virtOutputs = 0;
            switch (type)
            {
                case VoicemeeterType.Standard:
                    hardInputs = 2;
                    virtInputs = 1;
                    hardOutputs = 1;
                    virtOutputs = 1;
                    break;
                case VoicemeeterType.Banana:
                    hardInputs = 3;
                    virtInputs = 2;
                    hardOutputs = 3;
                    virtOutputs = 2;
                    break;
                case VoicemeeterType.Potato:
                case VoicemeeterType.Potato64:
                    hardInputs = 5;
                    virtInputs = 3;
                    hardOutputs = 5;
                    virtOutputs = 3;
                    break;
                default:
                    break;
            }

            osd.MainContent.Children.Clear();

            osd.AllowAutoUpdateSeparators = false;
            StripControl strip;
            for (int i = 0; i < hardInputs; i++)
            {
                strip = GetHardwareInputStrip(hardOutputs, virtOutputs);
                strip.StripLabel.Text = $"Hard In{i + 1}";
                osd.MainContent.Children.Add(strip);
            }
            for (int i = 0; i < virtInputs; i++)
            {
                strip = GetVirtualInputStrip(hardOutputs, virtOutputs);
                strip.StripLabel.Text = $"Virt In{i + 1}";
                osd.MainContent.Children.Add(strip);
            }
            for (int i = 0; i < hardOutputs; i++)
            {
                strip = GetOutputStrip(type);
                var name = hardOutputs == 1 ? $"A" : $"A{i + 1}";
                strip.StripLabel.Text = name;
                osd.MainContent.Children.Add(strip);
            }
            for (int i = 0; i < virtOutputs; i++)
            {
                strip = GetOutputStrip(type);
                var name = virtOutputs == 1 ? $"B" : $"B{i + 1}";
                strip.StripLabel.Text = name;
                osd.MainContent.Children.Add(strip);
            }
            osd.UpdateSeparators();
            osd.AllowAutoUpdateSeparators = true;
        }

        private static StripControl GetOutputStrip(VoicemeeterType type)
        {
            var strip = new StripControl();
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetMonoWithReverse());
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetMute());
            return strip;
        }

        private static StripControl GetInput(int physicalBuses, int virtualBuses)
        {
            var strip = new StripControl();
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetSolo());
            strip.ControlBtnsContainer.Children.Add(StripButtonFactory.GetMute());
            for (int i = 0; i < physicalBuses; i++)
            {
                var btnCont = StripButtonFactory.GetBusSelect();
                var name = (physicalBuses == 1) ? $"A" : $"A{i + 1}";
                btnCont.Btn.Content = name;
                strip.BusBtnsContainer.Children.Add(btnCont);
            }
            for (int i = 0; i < virtualBuses; i++)
            {
                var btnCont = StripButtonFactory.GetBusSelect();
                var name = (virtualBuses == 1) ? $"B" : $"B{i + 1}";
                btnCont.Btn.Content = name;
                strip.BusBtnsContainer.Children.Add(btnCont);
            }
            return strip;
        }

        private static StripControl GetVirtualInputStrip(int physicalBuses, int virtualBuses)
        {
            return GetInput(physicalBuses, virtualBuses);
        }

        public static StripControl GetHardwareInputStrip(int physicalBuses, int virtualBuses)
        {
            var strip = GetInput(physicalBuses, virtualBuses);
            strip.ControlBtnsContainer.Children.Insert(0, StripButtonFactory.GetMono());
            return strip;
        }
    }
}
