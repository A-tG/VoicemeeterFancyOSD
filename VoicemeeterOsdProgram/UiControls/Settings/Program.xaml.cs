using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Interop;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for Program.xaml
    /// </summary>
    public partial class Program : UserControl
    {
        public Dictionary<RenderMode, string> RmodeValues { get; } = new()
        {
            { RenderMode.Default, "Default" },
            { RenderMode.SoftwareOnly, "Software" }
        };

        public Program()
        {
            var o = OptionsStorage.Program;
            DataContext = o;

            InitializeComponent();
        }
    }
}
