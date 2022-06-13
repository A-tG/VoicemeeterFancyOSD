using AtgDev.Utils;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private PeriodicTimerExt m_movedTimer = new(TimeSpan.FromSeconds(2));
        private SettingsWindowViewModel m_model = new();

        public SettingsWindow()
        {
            DataContext = m_model;
            Initialized += OnInitialized;
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private async void OnInitialized(object sender, EventArgs e)
        {
            await m_model.TryReadWindowSettings();
        }

        private async void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsLoaded) return;

            m_movedTimer.Start();
            if (await m_movedTimer.WaitForNextTickAsync())
            {
                await m_model.TrySaveWindowSettings();
            }

        }

        private async void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            m_movedTimer.Stop();
            await m_model.TrySaveWindowSettings();
            // need to hide Window instead of closing becase TabControl keeps Window in memory (internal memory leak?)
            Hide();
        }

        private void OpenConfigFileClick(object sender, RoutedEventArgs e)
        {
            OpenInOs.TryOpen(OptionsStorage.ConfigFilePath);
        }

        private void OpenConfigFolderClick(object sender, RoutedEventArgs e)
        {
            string folder = Path.GetDirectoryName(OptionsStorage.ConfigFilePath);
            OpenInOs.TryOpen(folder);
        }
    }
}
