using System.Windows;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for Osd.xaml
    /// </summary>
    public partial class Osd : UserControl
    {
        public Osd()
        {
            DataContext = OptionsStorage.Osd;

            InitializeComponent();

            Unloaded += OnUnload;
        }


        private void OnUnload(object sender, RoutedEventArgs e)
        {
            return;
        }
    }
}
