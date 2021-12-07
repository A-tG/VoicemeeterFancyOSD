using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using VoicemeeterOsdProgram.Core;
using AtgDev.Utils;

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
            string url;
            if (await UpdateManager.TryCheckForUpdatesAsync())
            {
                DialogText.Text = msg + $"New version available: ";
                DialogText.Inlines.Add(GetVersionLink());
                url = UpdateManager.LatestRelease.HtmlUrl;
                UpdateBtn.IsEnabled = true;
            }
            else
            {
                DialogText.Text = msg + "No updates available";
                url = $"{UpdateManager.RepoUrl}/releases";
            }

            var link = GetWebLink();
            link.Click += (_, _) => OpenInOs.TryOpen(url);
            DialogText.Inlines.Add("\n");
            DialogText.Inlines.Add(link);
        }

        private Hyperlink GetVersionLink()
        {
            var ver = UpdateManager.LatestVersion.ToString();
            Hyperlink link = new(new Run(ver))
            {
                ToolTip = "Read release notes"
            };
            link.Click += OnVersionClick;
            return link;
        }

        private Hyperlink GetWebLink()
        {
            string url = UpdateManager.LatestRelease.HtmlUrl;
            Hyperlink link = new(new Run("GitHub Link"))
            {
                ToolTip = "Open GitHub webpage"
            };
            link.Click += (_, _) => OpenInOs.TryOpen(url);
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
