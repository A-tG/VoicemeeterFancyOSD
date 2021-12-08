using System.Windows;
using VoicemeeterOsdProgram.Updater;

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
            string title = "Release Notes";
            if (string.IsNullOrEmpty(UpdateManager.LatestRelease?.Body))
            {
                NotesBox.Text = "No release notes available";
            }
            else
            {
                NotesBox.Text = UpdateManager.LatestRelease.Body;
                title = $"{UpdateManager.LatestVersion} {title}";
            }
            Title = title;
        }

        private void OkClick(object sender, RoutedEventArgs e) => Close();

        private void Window_Loaded(object sender, RoutedEventArgs e) => GetReleaseNotes();
    }
}
