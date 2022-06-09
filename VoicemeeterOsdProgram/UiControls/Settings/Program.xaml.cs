using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for Program.xaml
    /// </summary>
    public partial class Program : StackPanel
    {
        public Dictionary<RenderMode, string> RmodeValues { get; } = new()
        {
            { RenderMode.Default, "Default" },
            { RenderMode.SoftwareOnly, "Software" }
        };

        public Program()
        {
            InitializeComponent();

            var o = OptionsStorage.Program;

            AutostartChbox.IsChecked = o.Autostart;
            RenderingModeCombo.SelectedValue = o.RenderMode;

            o.AutostartChanged += OptionEvent_Autostart;
            o.RenderModeChanged += OptionEvent_RenderMode;

            AutostartChbox.Click += (_, _) => o.Autostart = AutostartChbox.IsChecked ?? false;
            RenderingModeCombo.SelectionChanged += RenderingModeSelected;

            Unloaded += OnUnload;
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            var o = OptionsStorage.Program;
            o.AutostartChanged -= OptionEvent_Autostart;
            o.RenderModeChanged -= OptionEvent_RenderMode;
        }

        private void RenderingModeSelected(object sender, SelectionChangedEventArgs e)
        {
            var items = e.AddedItems.OfType<KeyValuePair<RenderMode, string>>().ToArray();
            if (items.Length == 0) return;

            OptionsStorage.Program.RenderMode = items[0].Key;
        }

        private void OptionEvent_Autostart(object o, bool val) => AutostartChbox.IsChecked = val;

        private void OptionEvent_RenderMode(object o, RenderMode val) => RenderingModeCombo.SelectedValue = val;
    }
}
