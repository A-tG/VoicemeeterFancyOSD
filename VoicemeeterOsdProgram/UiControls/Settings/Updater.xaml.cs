using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for Updater.xaml
    /// </summary>
    public partial class Updater : UserControl
    {
        public Updater()
        {
            DataContext = OptionsStorage.Updater;
            InitializeComponent();
        }
    }
}
