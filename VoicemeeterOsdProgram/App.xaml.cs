using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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

            Current.DispatcherUnhandledException += OnUnhandledException;
        }

        async void OnAppStartup(object sender, StartupEventArgs e)
        {
            var optionsTask = OptionsStorage.InitAsync();

            RenderOptions.ProcessRenderMode = OptionsStorage.Program.RenderMode;
            OptionsStorage.Program.RenderModeChanged += (_, val) => RenderOptions.ProcessRenderMode = val;

            VoicemeeterApiClient.PoolingRate = OptionsStorage.Voicemeeter.ApiPoolingRate;
            OptionsStorage.Voicemeeter.ApiPoolingRateChanged += (_, val) => VoicemeeterApiClient.PoolingRate = val;

            DpiHelper.Init();
            TrayIconManager.Init();
            OsdWindowManager.Init();
            UpdateManager.DefaultOS = System.Runtime.InteropServices.OSPlatform.Windows;

            await optionsTask;
            var vmTask = VoicemeeterApiClient.InitAsync((int)OptionsStorage.Voicemeeter.InitializationDelay);

            if (OptionsStorage.Updater.CheckOnStartup)
            {
                var updaterRes = await UpdateManager.TryCheckForUpdatesAsync();
                if (updaterRes == UpdaterResult.NewVersionFound)
                {
                    TrayIconManager.OpenUpdater();
                }
            }

            await vmTask;
            await ArgsHandler.HandleAsync(AppLifeManager.appArgs);
            // start to recieve command-line arguments from other launched instance
            AppLifeManager.StartArgsPipeServer();

            Globals.logger?.Log("Program initialized");

            await CheckProgramDirectoryIOAsync();
        }

        private async Task CheckProgramDirectoryIOAsync()
        {
            const string Msg = "Unable to create files/directories in the program's directory.\n" + 
                "Updater and persistent config might not work.\n\n" + 
                "Possible solution: if program is located in 'Program Files' move it to a different folder/drive";

            string path = AppDomain.CurrentDomain.BaseDirectory;
            bool canCreateDirs = IOAccessCheck.TryCreateRandomDirectory(path);
            bool canCreateFiles = await IOAccessCheck.TryCreateRandomFileAsync(path);
            if (!canCreateDirs || !canCreateFiles)
            {
                var exType = IOAccessCheck.LastException.GetType();
                var d = MsgBoxFactory.GetWarning();
                d.ContentToDisplay.Content = $"{exType}\n{Msg}";
                d.Show();
            }
        }

        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Globals.logger?.LogCritical($"Unhandled exception: {e.Exception}");
        }
    }
}
