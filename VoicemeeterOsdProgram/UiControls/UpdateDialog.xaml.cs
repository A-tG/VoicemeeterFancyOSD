using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using VoicemeeterOsdProgram.Core;

namespace VoicemeeterOsdProgram.UiControls
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {
        private UpdateReleaseNotes m_relNotesWin;

        public UpdateDialog()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _ = CheckVersion();
        }

        private async Task CheckVersion()
        {
            var msg = $"Current: {UpdateManager.CurrentVersion}\n";
            if (await UpdateManager.TryCheckForUpdatesAsync())
            {
                DialogText.Text = msg + $"New version available: ";
                DialogText.Inlines.Add(GetVersionLink());
                UpdateBtn.IsEnabled = true;
            }
            else
            {
                DialogText.Text = msg + "No updates available";
            }
        }

        private Hyperlink GetVersionLink()
        {
            var ver = UpdateManager.LatestVersion.ToString();
            Hyperlink link = new(new Run(ver))
            {
                ToolTip = "Read release notes",
                NavigateUri = new Uri(UpdateManager.LatestRelease.HtmlUrl)
            };
            link.Click += OnVersionClick;
            return link;
        }

        private void OnVersionClick(object sender, RoutedEventArgs e)
        {
            if (m_relNotesWin is null)
            {
                m_relNotesWin = new()
                {
                    Owner = this,
                };
                m_relNotesWin.Closing += (_, _) => m_relNotesWin = null;
            }
            m_relNotesWin.Show();
            m_relNotesWin.Activate();
        }

        private void CloseClick(object sender, RoutedEventArgs e) => Close();

        private async void UpdateClick(object sender, RoutedEventArgs e)
        {
            UpdateBtn.IsEnabled = false;
            DialogText.Text = "Updating...";
            if (!await UpdateManager.TryUpdate())
            {
                DialogText.Text = "Update failed";
            }
            UpdateBtn.IsEnabled = true;
        }
    }
}
