using AtgDev.Utils;
using System;
using System.IO;
using System.Windows;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.UiControls.Settings;

namespace VoicemeeterOsdProgram.UiControls.Tray
{
    /// <summary>
    /// Interaction logic for TrayIcon.xaml
    /// </summary>
    public partial class TrayIcon : Window
    {
        private UpdateDialog m_updateDialog;
        private Window m_settingsWindow;
        private bool m_isPaused = false;

        public TrayIcon()
        {
            InitializeComponent();
            IsPaused = OptionsStorage.Other.Paused;
            OptionsStorage.Other.PausedChanged += (_, val) => IsPaused = val;

#if DEBUG
            DebugWindowItem.Visibility = Visibility.Visible;
            DebugWindowItem.Click += OnDebugWindowClick;
#endif
        }

        public bool IsPaused
        {
            get => m_isPaused;
            set
            {
                if (m_isPaused == value) return;

                m_isPaused = value;
                TogglePaused(value);
            }
        }

        public void CheckForUpdate()
        {
            if (m_updateDialog is null)
            {
                m_updateDialog = new();
                m_updateDialog.Closing += (_, _) => m_updateDialog = null;
            }
            m_updateDialog.Show();
            m_updateDialog.Activate();
        }

        private void TogglePaused(bool val)
        {
            if (val)
            {
                NotifyIcon.Icon = Properties.Resources.MainIconInactive;
                PausedItem.FontWeight = FontWeights.Bold;
            }
            else
            {
                NotifyIcon.Icon = Properties.Resources.MainIcon;
                PausedItem.FontWeight = FontWeights.Normal;
            }
            OptionsStorage.Other.Paused = val;
            PausedItem.IsChecked = val;
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e) => OpenSettingsWindow();

        private void OnOpenConfigClick(object sender, RoutedEventArgs e)
        {
            OpenInOs.TryOpen(OptionsStorage.ConfigFilePath);
        }

        private void OnOpenConfigFolderClick(object sender, RoutedEventArgs e)
        {
            string folder = Path.GetDirectoryName(OptionsStorage.ConfigFilePath);
            OpenInOs.TryOpen(folder);
        }

        private void CheckForUpdateClick(object sender, RoutedEventArgs e)
        {
            CheckForUpdate();
        }

        private void OnPausedClick(object sender, RoutedEventArgs e)
        {
            IsPaused = !IsPaused;
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            IsPaused = !IsPaused;
        }

        private void OpenSettingsWindow()
        {
            if (m_settingsWindow is null)
            {
                m_settingsWindow = new SettingsWindow();
            }
            m_settingsWindow.Show();
            m_settingsWindow.Activate();
        }

#if DEBUG
        private DebugWindow m_debugWin;

        private void OnDebugWindowClick(object sender, EventArgs e)
        {
            if (m_debugWin is null)
            {
                m_debugWin = new DebugWindow();
                m_debugWin.Closing += OnDebugWin_Closing;
            }
            m_debugWin.Show();
            m_debugWin.Activate();
        }

        private void OnDebugWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            m_debugWin.Hide();
        }
#endif
    }
}
