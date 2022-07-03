using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for Logger.xaml
    /// </summary>
    public partial class Logger : UserControl
    {
        public Logger()
        {
            DataContext = OptionsStorage.Logger;
            InitializeComponent();
        }
    }
}
