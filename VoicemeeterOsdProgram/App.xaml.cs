using System.Windows;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Helpers;
using VoicemeeterOsdProgram.Options;

namespace VoicemeeterOsdProgram
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        void OnAppStartup(object sender, StartupEventArgs e)
        {
            OptionsStorage.Init();
            DpiHelper.Init();
            ScreenWorkingAreaManager.Init();
            TrayIconManager.Init();
            VoicemeeterApiClient.Init();
            OsdWindowManager.Init();
            if (OptionsStorage.Updater.CheckAutomatically)
            {
                _ = UpdateManager.TryCheckForUpdatesAsync();
            }
        }
    }
}
