﻿using Atg.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using TopmostApp.Helpers;
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

        void OnAppStartup(object sender, StartupEventArgs e)
        {
            _ = Init();
        }

        private async Task Init()
        {
            UpdateManager.DefaultOS = System.Runtime.InteropServices.OSPlatform.Windows;
            try
            {
                FullscreenAppsWatcher.appsToDetect = new ListInFile(@$"{AppDomain.CurrentDomain.BaseDirectory}config\detect_apps.txt")
                {
                    AllowDuplicates = false,
                    IsCaseSensetive = false,
                };
                var AltOsdOpt = OptionsStorage.AltOsdOptionsFullscreenApps;
                FullscreenAppsWatcher.IsEnabled = AltOsdOpt.Enabled;
                AltOsdOpt.EnabledChanged += (_, val) => FullscreenAppsWatcher.IsEnabled = val;
            }
            catch { }
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
