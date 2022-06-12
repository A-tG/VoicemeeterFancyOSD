using AtgDev.Utils;
using System.IO;
using System.Windows;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            Closing += SettingsWindow_Closing;
            InitializeComponent();
        }

        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // need to hide Window instead of closing becase TabControl keeps Window in memory (internal memory leak?)
            e.Cancel = true;
            Hide();
        }

        private void OpenConfigFileClick(object sender, RoutedEventArgs e)
        {
            OpenInOs.TryOpen(OptionsStorage.ConfigFilePath);
        }

        private void OpenConfigFolderClick(object sender, RoutedEventArgs e)
        {
            string folder = Path.GetDirectoryName(OptionsStorage.ConfigFilePath);
            OpenInOs.TryOpen(folder);
        }
    }
}
