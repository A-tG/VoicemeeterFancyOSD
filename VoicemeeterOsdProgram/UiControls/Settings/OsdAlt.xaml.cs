using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for OsdAlt.xaml
    /// </summary>
    public partial class OsdAlt : UserControl
    {
        public OsdAlt()
        {
            DataContext = OptionsStorage.AltOsdOptionsFullscreenApps;
            InitializeComponent();
        }
    }
}
