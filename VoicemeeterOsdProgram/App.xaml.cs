using Atg.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using TopmostApp.Helpers;
using VoicemeeterOsdProgram.Core;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Helpers;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Updater;
using VoicemeeterOsdProgram.Updater.Types;

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
            _ = Init();
        }

        private async Task Init()
        {
            UpdateManager.DefaultOS = System.Runtime.InteropServices.OSPlatform.Windows;
            OptionsStorage.Init();
            DpiHelper.Init();
            TrayIconManager.Init();
            VoicemeeterApiClient.Init();
            OsdWindowManager.Init();

            var updaterRes = await UpdateManager.TryCheckForUpdatesAsync();
            if (OptionsStorage.Updater.CheckOnStartup && (updaterRes == UpdaterResult.NewVersionFound))
            {
                TrayIconManager.OpenUpdaterWindow();
            }
        }
    }
}
