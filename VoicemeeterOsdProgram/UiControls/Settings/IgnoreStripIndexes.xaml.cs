using AtgDev.Voicemeeter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoicemeeterOsdProgram.Core.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for IgnoreStripIndexes.xaml
    /// </summary>
    public partial class IgnoreStripIndexes : UserControl
    {
        public IgnoreStripIndexes()
        {
            InitializeComponent();
            InitList();
        }

        private void InitList()
        {
            GridView gv = new();
            VoicemeeterProperties[] vmProps = {
                new VoicemeeterProperties(VoicemeeterType.Standard),
                new VoicemeeterProperties(VoicemeeterType.Banana),
                new VoicemeeterProperties(VoicemeeterType.Potato)
            };
            int dataColumnsNumber = vmProps.Length;

            var indexColumn = new GridViewColumn()
            {
                DisplayMemberBinding = new Binding($"[{0}]")
            };
            gv.Columns.Add(indexColumn);

            for (int i = 0; i < dataColumnsNumber; i++)
            {
                var c = new GridViewColumn()
                {
                    Header = vmProps[i].type.ToString(),
                    DisplayMemberBinding = new Binding($"[{i + 1}]")
                };
                gv.Columns.Add(c);
            }

            ListViewControl.View = gv;

            var biggest = vmProps.MaxBy(p => p.hardInputs + p.virtInputs + p.hardOutputs + p.virtOutputs);
            var numberOfEntries = biggest.hardInputs + biggest.virtInputs + biggest.hardOutputs + biggest.virtOutputs;
            for (int i = 0; i < numberOfEntries; i++)
            {
                List<string> itemData = new();
                itemData.Add(i.ToString());
                for (int j = 0; j < dataColumnsNumber; j++)
                {
                    itemData.Add(GetName(vmProps[j], i));
                }
                ListViewControl.Items.Add(itemData);
            }
        }

        private string GetName(VoicemeeterProperties p, int index)
        {
            index++;
            string name = "";
            int HV = p.hardInputs + p.virtInputs;
            int HVA = HV + p.hardOutputs;
            int HVAB = HVA + p.virtOutputs;

            if (index <= p.hardInputs)
            {
                name = $"HwIn {index}";
            }
            else if (index <= HV)
            {
                int virtInpIndex = index - p.hardInputs;
                name = virtInpIndex switch
                {
                    <= 1 => "VAIO",
                    <= 2 => "AUX",
                    <= 3 => "VAIO 3",
                    _ => $"VirtIn {virtInpIndex}"
                };
            }
            else if (index <= HVA)
            {
                name = (p.type == VoicemeeterType.Standard) ? "A" : $"A{index - HV}";
            }
            else if (index <= HVAB)
            {
                name = (p.type == VoicemeeterType.Standard) ? "B" : $"B{index - HVA}";
            }
            return name;
        }
    }
}
