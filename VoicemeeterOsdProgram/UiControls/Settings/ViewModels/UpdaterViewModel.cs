using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using VoicemeeterOsdProgram.Types;
using VoicemeeterOsdProgram.Updater;
using VoicemeeterOsdProgram.Updater.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings.ViewModels
{
    public class UpdaterViewModel : BaseViewModel
    {
        private enum State
        {
            Initial,
            CheckingVersion,
            NewVersionFound,
            Connecting,
            Downloading,
            Extracting,
            Installing,
            Done
        }

        private double m_progressVal = 0;
        private State m_state;
        private string m_progressText = "", m_infoText, m_relNotes, m_buttonText = "Check for Updates";
        private bool m_isEnabled = true, m_isInProgress = false, m_isRelNotesEnabled;

        public UpdaterViewModel()
        {
            m_infoText = $"Press '{m_buttonText}'...";
            CurrentState = State.Initial;
            if (UpdateManager.LastResult == UpdaterResult.NewVersionFound)
            {
                CurrentState = State.NewVersionFound;
            }
            BtnCommand = new RelayCommand(_ => OnUpdateBtn());
        }

        public string VersionText => $"Current version: {UpdateManager.CurrentVersion}";

        public double ProgressValue
        {
            get => m_progressVal;
            set
            {
                m_progressVal = value;
                OnPropertyChanged();
            }
        }

        public string ProgressText
        {
            get => m_progressText;
            set
            {
                m_progressText = value;
                OnPropertyChanged();
            }
        }

        public string InfoText
        {
            get => m_infoText;
            set
            {
                m_infoText = value;
                OnPropertyChanged();
            }
        }

        public string ButtonText
        {
            get => m_buttonText;
            set
            {
                m_buttonText = value;
                OnPropertyChanged();
            }
        }

        public bool IsInProgress
        {
            get => m_isInProgress;
            set
            {
                m_isInProgress = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => m_isEnabled;
            set
            {
                m_isEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsRelNotesEnabled
        {
            get => m_isRelNotesEnabled;
            set
            {
                m_isRelNotesEnabled = value;
                if (value)
                {
                    RelNotes = string.IsNullOrEmpty(UpdateManager.LatestRelease?.Body) ? 
                        "No release notes available" : 
                        UpdateManager.LatestRelease.Body;
                }
                OnPropertyChanged();
            }
        }

        public string RelNotes
        {
            get => m_relNotes;
            set
            {
                m_relNotes = value;
                OnPropertyChanged();
            }
        }


        private State CurrentState
        {
            get => m_state;
            set
            {
                m_state = value;
                bool isEnabled = true;
                switch (value)
                {
                    case State.Initial:
                        IsInProgress = false;
                        ProgressValue = 0;
                        ProgressText = "";
                        ButtonText = "Check for Updates";
                        break;
                    case State.CheckingVersion:
                        isEnabled = false;
                        ButtonText = "Checking...";
                        break;
                    case State.Connecting:
                        isEnabled = false;
                        ButtonText = "Connecting...";
                        break;
                    case State.NewVersionFound:
#if DEBUG
                        InfoText = $"Version available: {UpdateManager.LatestVersion}";
#else
                        InfoText = $"New version available: {UpdateManager.LatestVersion}";
#endif
                        ButtonText = "Update";
                        break;
                    case State.Downloading:
                    case State.Extracting:
                        IsInProgress = true;
                        ButtonText = "Cancel";
                        break;
                    case State.Installing:
                        isEnabled = false;
                        InfoText = "Installing...";
                        break;
                    case State.Done:
                        isEnabled = false;
                        InfoText = "Done, restarting...";
                        break;
                    default:
                        break;
                }
                IsEnabled = isEnabled;
            }
        }

        public ICommand BtnCommand { get; }

        private async void OnUpdateBtn()
        {
            switch (CurrentState)
            {
                case State.Initial:
                    await CheckNewVersion();
                    break;
                case State.NewVersionFound:
                    await Update();
                    break;
                case State.Downloading:
                case State.Extracting:
                    Cancel();
                    break;
                case State.Installing:
                    break;
                default:
                    break;
            }
        }

        private async Task CheckNewVersion()
        {
            CurrentState = State.CheckingVersion;
            var res = await UpdateManager.TryCheckForUpdatesAsync();
            HandleUpdaterRes(res);
        }

        private async Task Update()
        {
            CurrentState = State.Connecting;
            var downloadP = new Progress<CurrentTotalBytes>(DownProgrChanged);
            var extractP = new Progress<double>(ExtrProgrChanged);
            var copyP = new Progress<double>(InstProgrChanged);

            var res = await UpdateManager.TryUpdateAsync(downloadP, extractP, copyP);
            HandleUpdaterRes(res);
        }

        private void Cancel() => UpdateManager.CancelUpdate();

        private void HandleUpdaterRes(UpdaterResult res)
        {
            switch (res)
            {
                case UpdaterResult.Error:
                    InfoText = "Unknown error";
                    break;
                case UpdaterResult.Updated:
                    CurrentState = State.Done;
                    return;
                case UpdaterResult.NewVersionFound:
                    CurrentState = State.NewVersionFound;
                    IsRelNotesEnabled = true;
                    return;
                case UpdaterResult.VersionUpToDate:
                    InfoText = "You're running the latest version";
                    IsRelNotesEnabled = true;
                    break;
                case UpdaterResult.ConnectionError:
                case UpdaterResult.ArchitectureNotFound:
                case UpdaterResult.OsNotFound:
                case UpdaterResult.ReleasesNotFound:
                case UpdaterResult.UpdateFailed:
                case UpdaterResult.DownloadFailed:
                case UpdaterResult.ArchiveExtractionFailed:
                    InfoText = $"Error: {res}";
                    break;
                case UpdaterResult.Canceled:
                    InfoText = "Canceled";
                    break;
                default:
                    break;
            }
            CurrentState = State.Initial;
        }

        private void DownProgrChanged(CurrentTotalBytes val)
        {
            CurrentState = State.Downloading;
            var currentKb = val.Current / 1024;
            var totalKb = val.Total / 1024;
            ProgressValue = val.ProgressPercent;
            ProgressText = $"{currentKb} / {totalKb} kB";
        }

        private void ExtrProgrChanged(double val)
        {
            CurrentState = State.Extracting;
            ProgressValue = val;
            ProgressText = $"{val}%";
        }

        private void InstProgrChanged(double val)
        {
            CurrentState = State.Installing;
            ProgressValue = val;
            ProgressText = $"{val}%";
        }
    }
}
