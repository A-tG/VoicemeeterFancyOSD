using AtgDev.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Types;
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

        public TrayIcon()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Destroy();

            InitializeComponent();
            NotifyIcon.LeftClickCommand = new DelegateCommand(_ => OpenSettingsWindow());
#if DEBUG
            DebugWindowItem.Visibility = Visibility.Visible;
            DebugWindowItem.Click += OnDebugWindowClick;
#endif
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
            var mItem = (MenuItem)sender;
            mItem.IsChecked = !mItem.IsChecked;
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            PausedItem.IsChecked = !PausedItem.IsChecked;
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


        private void Destroy()
        {
            System.Diagnostics.Debug.WriteLine("Disposing Tray Icon");
            App.Current.Dispatcher.Invoke(() => NotifyIcon?.Dispose());
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
