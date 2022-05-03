using System;
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
            var optionsInit = OptionsStorage.InitAsync();
            Task[] tasks = {
                VoicemeeterApiClient.InitAsync(), 
                optionsInit 
            };

            DpiHelper.Init();
            TrayIconManager.Init();
            OsdWindowManager.Init();
            UpdateManager.DefaultOS = System.Runtime.InteropServices.OSPlatform.Windows;

            await optionsInit;
            if (OptionsStorage.Updater.CheckOnStartup)
            {
                var updaterRes = await UpdateManager.TryCheckForUpdatesAsync();
                if (updaterRes == UpdaterResult.NewVersionFound)
                {
                    TrayIconManager.OpenUpdaterWindow();
                }
            }
            await Task.WhenAll(tasks);
            await ArgsHandler.HandleAsync(AppLifeManager.appArgs);
            // start to recieve command-line arguments from other launched instance
            AppLifeManager.StartArgsPipeServer();

            Globals.Init(); // to initialize static fields

            await CheckProgramDirectoryIO();
        }

        private async Task CheckProgramDirectoryIO()
        {
            const string Msg = "Unable to create files/directories in the program's directory. Updater and persistent config may not work." + 
                "\nPossible solution: if program is located in Program Files move it to a different folder/drive";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            bool canCreateDirs = IOAccessCheck.TryCreateRandomDirectory(path);
            bool canCreateFiles = await IOAccessCheck.TryCreateRandomFileAsync(path);
            if (!canCreateDirs || !canCreateFiles)
            {
                var exType = IOAccessCheck.LastException.GetType();
                await Task.Run(() => MessageBox.Show($"{exType}\n{Msg}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning));
            }
        }
    }
}
