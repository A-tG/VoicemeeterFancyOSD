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
            string text;
            string title;
            if (string.IsNullOrEmpty(UpdateManager.LatestRelease?.Body))
            {
                text = "No release notes available";
                title = "Release Notes";
            }
            else
            {
                text = UpdateManager.LatestRelease.Body;
                title = $"{UpdateManager.LatestVersion} Release Notes";
            }
            NotesBox.Text = text;
            Title = title;
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
