using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using VoicemeeterOsdProgram.Core;
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

        async void OnAppStartup(object sender, StartupEventArgs e)
        {
            List<Task> tasks = new();

            var OptionsInitTask = OptionsStorage.InitAsync();
            tasks.Add(OptionsInitTask);
            tasks.Add(VoicemeeterApiClient.InitAsync());

            DpiHelper.Init();
            TrayIconManager.Init();
            OsdWindowManager.Init();
            UpdateManager.DefaultOS = System.Runtime.InteropServices.OSPlatform.Windows;

            await OptionsInitTask;
            if (OptionsStorage.Updater.CheckOnStartup)
            {
                var updaterRes = await UpdateManager.TryCheckForUpdatesAsync();
                if (updaterRes == UpdaterResult.NewVersionFound)
                {
                    TrayIconManager.OpenUpdaterWindow();
                }
            }
            await Task.WhenAll(tasks);
        }
    }
}
