using AtgDev.Voicemeeter.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(
            nameof(Values), typeof(ImmutableHashSet<uint>), typeof(IgnoreStripIndexes));
        public ImmutableHashSet<uint> Values
        {
            get => (ImmutableHashSet<uint>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }

        private void InitList()
        {
            GridView gv = new GridView();
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

        private static string GetName(VoicemeeterProperties p, int index)
        {
            var indexToDisplay = index + 1;
            string name = "";
            int HV = p.hardInputs + p.virtInputs;
            int HVA = HV + p.hardOutputs;
            int HVAB = HVA + p.virtOutputs;

            if (index < p.hardInputs)
            {
                name = $"HwIn {indexToDisplay}";
            }
            else if (index < HV)
            {
                int virtInpIndex = index - p.hardInputs;
                name = virtInpIndex switch
                {
                    < 1 => "VAIO",
                    < 2 => "AUX",
                    < 3 => "VAIO 3",
                    _ => $"VirtIn {virtInpIndex}"
                };
            }
            else if (index < HVA)
            {
                name = (p.type == VoicemeeterType.Standard) ? "A" : $"A{indexToDisplay - HV}";
            }
            else if (index < HVAB)
            {
                name = (p.type == VoicemeeterType.Standard) ? "B" : $"B{indexToDisplay - HVA}";
            }
            return name;
        }

        private HashSet<uint> GetValuesFromTextbox()
        {
            HashSet<uint> set = new();
            var values = TextBoxControl.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var val in values)
            {
                if (uint.TryParse(val, out uint numb))
                {
                    set.Add(numb);
                }
            }
            return set;
        }

        private HashSet<uint> GetSelectedValues()
        {
            HashSet<uint> set = new();
            var items = ListViewControl.SelectedItems;
            if (items is null) return set;

            foreach (var val in items)
            {
                if (val is not List<string> item) continue;
                if (item.Count == 0) continue;
                
                if (uint.TryParse(item[0], out uint numb))
                {
                    set.Add(numb);
                }
            }
            return set;
        }

        private void GetValues()
        {
            var vals = GetValuesFromTextbox();
            vals.UnionWith(GetSelectedValues());
            Values = vals.ToImmutableHashSet();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) => GetValues();

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("changed");
            var text = TextBoxControl.Text;
            if (text.Contains(' '))
            {
                TextBoxControl.Text = text.Replace(" ", "");
                return;
            }
            GetValues();
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            foreach (var ch in e.Text)
            {
                if (!(char.IsDigit(ch) || (ch == ',')))
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = false;
        }
    }
}
