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
using System.Windows.Shapes;
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
            if (UpdateManager.IsNewVersionAvailable || (await UpdateManager.TryCheckForUpdatesAsync()))
            {
                DialogText.Text = $"New version available: {UpdateManager.LatestVersion}";
                UpdateBtn.IsEnabled = true;
            } else
            {
                DialogText.Text = "No updates available";
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void UpdateClick(object sender, RoutedEventArgs e)
        {
            await UpdateManager.TryUpdate();
        }
    }
}
