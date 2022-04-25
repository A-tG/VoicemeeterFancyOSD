using AtgDev.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using VoicemeeterOsdProgram.Updater;
using VoicemeeterOsdProgram.Updater.Types;

namespace VoicemeeterOsdProgram.UiControls
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {
        private UpdateReleaseNotes m_relNotesWin;
        private bool m_isUpdating = false;
        private bool m_isCopying = false;

        private bool IsUpdating
        {
            get => m_isUpdating;
            set
            {
                m_isUpdating = value;
                CloseBtn.Content = value ? "Cancel" : "Close";
                UpdateBtn.IsEnabled = !value;
            }
        }

        private bool IsCopying
        {
            get => m_isCopying;
            set
            {
                m_isCopying = value;
                CloseBtn.IsEnabled = !value;
            }
        }

        public UpdateDialog()
        {
            InitializeComponent();
            Closing += UpdateDialog_Closing;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _ = CheckVersion();
        }

        private async Task CheckVersion()
        {
            ProcessUpdaterResult(await UpdateManager.TryCheckForUpdatesAsync());
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

        private Hyperlink GetWebLink(string url)
        {
            Hyperlink link = new(new Run("GitHub Link"))
            {
                ToolTip = "Open GitHub webpage"
            };
            link.Click += (_, _) => OpenInOs.TryOpen(url);
            return link;
        }

        private void ProcessUpdaterResult(UpdaterResult res)
        {
            var arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
            var updateBtnText = "Try Update";
            var msg = $"Current version: {arch} {UpdateManager.CurrentVersion}\n";
            var url = $"{UpdateManager.RepoUrl}/releases";
            bool isUpdating = false;

            ProgrBar.Visibility = Visibility.Collapsed;
            switch (res)
            {
                case UpdaterResult.Error:
                    DialogText.Text = msg + "Unknown error";
                    break;
                case UpdaterResult.Updated:
                    DialogText.Text = msg + "Updated, restarting the program...";
                    isUpdating = true;
                    break;
                case UpdaterResult.NewVersionFound:
#if DEBUG
                    var updateMsg = "Version available: ";
#else
                    var updateMsg = "New version available: ";
#endif
                    DialogText.Text = msg + updateMsg;
                    DialogText.Inlines.Add(GetVersionLink());
                    url = UpdateManager.LatestRelease.HtmlUrl;
                    break;
                case UpdaterResult.VersionUpToDate:
                    DialogText.Text = msg + "You're running the latest version";
                    break;
                case UpdaterResult.ConnectionError:
                    DialogText.Text = msg + "Error: unable to connect to the server";
                    break;
                case UpdaterResult.ArchitectureNotFound:
                    DialogText.Text = msg + "Error: no suitable architecture found";
                    break;
                case UpdaterResult.ReleasesNotFound:
                    DialogText.Text = msg + "Error: no releases found";
                    break;
                case UpdaterResult.UpdateFailed:
                    DialogText.Text = msg + "Error: update failed";
                    break;
                case UpdaterResult.DownloadFailed:
                    DialogText.Text = msg + "Error: download failed";
                    break;
                case UpdaterResult.ArchiveExtractionFailed:
                    DialogText.Text = msg + "Error: unpacking failed";
                    break;
                case UpdaterResult.Canceled:
                    DialogText.Text = msg + "Canceled";
                    break;
                default:
                    break;
            }

            var link = GetWebLink(url);
            DialogText.Inlines.Add("\n");
            DialogText.Inlines.Add(link);
            IsUpdating = isUpdating;
            UpdateBtn.Content = updateBtnText;
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

        private void UpdateDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateManager.CancelUpdate();
            if (IsCopying)
            {
                e.Cancel = true;
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            if (IsUpdating)
            {
                if (IsCopying) return;

                UpdateManager.CancelUpdate();
                ProgrBar.Value = 0;
                IsUpdating = false;
            }
            else
            {
                Close();
            }
        }

        private async void UpdateClick(object sender, RoutedEventArgs e)
        {
            IsUpdating = true;

            var downloadP = new Progress<CurrentTotalBytes>(DownProgrChanged);
            var extractP = new Progress<double>(ExtrProgrChanged);
            var copyP = new Progress<double>(CopyProgrChanged);

            ProcessUpdaterResult(await UpdateManager.TryUpdateAsync(downloadP, extractP, copyP));
            IsCopying = false;
        }

        private void DownProgrChanged(CurrentTotalBytes val)
        {
            ProgrBar.Visibility = Visibility.Visible;

            var currentKb = val.Current / 1024;
            var totalKb = val.Total / 1024;
            ProgrBar.Value = val.ProgressPercent;
            ProgrBarText.Text = $"{currentKb} / {totalKb} KB";

            DialogText.Text = "Downloading...";
        }

        private void ExtrProgrChanged(double val)
        {
            ProgrBar.Visibility = Visibility.Visible;
            ProgrBar.Value = val;
            ProgrBarText.Text = "";
            DialogText.Text = "Extracting...";
        }

        private void CopyProgrChanged(double val)
        {
            IsCopying = true;
            ProgrBar.Visibility = Visibility.Visible;
            ProgrBar.Value = val;
            ProgrBarText.Text = "";
            DialogText.Text = "Copying...";
        }
    }
}
