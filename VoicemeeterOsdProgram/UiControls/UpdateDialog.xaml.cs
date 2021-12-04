using System.Windows;
using VoicemeeterOsdProgram.Core;

namespace VoicemeeterOsdProgram.UiControls
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {
        public UpdateDialog()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var msg = $"Current: {UpdateManager.CurrentVersion}\n";
            if (await UpdateManager.TryCheckForUpdatesAsync())
            {
                DialogText.Text = msg + $"New version available: {UpdateManager.LatestVersion}";
                UpdateBtn.IsEnabled = true;
            } else
            {
                DialogText.Text = msg + "No updates available";
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void UpdateClick(object sender, RoutedEventArgs e)
        {
            if (!await UpdateManager.TryUpdate())
            {
                DialogText.Text = "Update failed";
            }
        }
    }
}
