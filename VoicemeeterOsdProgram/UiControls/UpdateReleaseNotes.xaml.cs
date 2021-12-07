using System.Windows;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Core;

namespace VoicemeeterOsdProgram.UiControls
{
    /// <summary>
    /// Interaction logic for UpdateReleaseNotes.xaml
    /// </summary>
    public partial class UpdateReleaseNotes : Window
    {
        public UpdateReleaseNotes()
        {
            InitializeComponent();
        }

        public void GetReleaseNotes()
        {
            TextBox.Text = string.IsNullOrEmpty(UpdateManager.LatestRelease?.Body) ?
                "No release notes available" : 
                UpdateManager.LatestRelease.Body;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetReleaseNotes();
        }
    }
}
