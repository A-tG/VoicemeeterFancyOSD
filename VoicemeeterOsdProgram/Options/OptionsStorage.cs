using AtgDev.Utils;
using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VoicemeeterOsdProgram.Options
{
    public static class OptionsStorage
    {
        public static readonly OsdOptions Osd = new();
        public static readonly OsdAlternative AltOsdOptionsFullscreenApps = new();
        public static readonly UpdaterOptions Updater = new();
        public static readonly OtherOptions Other = new();

        private static readonly string m_path = @$"{AppDomain.CurrentDomain.BaseDirectory}config\config.ini";

        private static readonly IniDataParser m_parser = new();
        private static IniData m_data = new();
        private static FileSystemWatcher m_watcher = new();
        private static PeriodicTimerExt m_timer = new(TimeSpan.FromSeconds(1));
        private static bool m_isWatcherEnabled;
        private static bool m_isWatcherPaused;
        private static bool m_isInit = false;

        static OptionsStorage()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            System.Windows.Application.Current.Exit += (_, _) => Exit();
        }

        public static string ConfigFilePath => m_path;

        public static async Task InitAsync()
        {
            if (m_isInit) return;

            await ValidateConfigFileAsync();

            m_watcher.Path = Path.GetDirectoryName(m_path);
            m_watcher.Filter = Path.GetFileName(m_path);
            m_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            m_watcher.Changed += OnConfigFileChanged;
            IsWatcherEnabled = true;

            m_isInit = true;
        }

        public static bool IsWatcherEnabled
        {
            get => m_isWatcherEnabled;
            set
            {
                m_isWatcherEnabled = value;
                if (value)
                {
                    m_watcher.EnableRaisingEvents = !IsWatcherPaused;
                    if (IsWatcherPaused)
                    {
                        m_timer.Stop();
                    }
                }
                else
                {
                    m_watcher.EnableRaisingEvents = false;
                    m_timer.Stop();
                }
            }
        }

        public static bool IsWatcherPaused
        {
            get => m_isWatcherPaused;
            set
            {
                m_isWatcherPaused = value;
                if (value)
                {
                    m_watcher.EnableRaisingEvents = false;
                    m_timer.Stop();
                }
                else
                {
                    m_watcher.EnableRaisingEvents = IsWatcherEnabled;
                    if (!IsWatcherEnabled)
                    {
                        m_timer.Stop();
                    }
                }
            }
        }

        public static async Task<bool> TrySaveAsync()
        {
            bool result = false;
            IsWatcherPaused = true;

            try
            {
                var directoryPath = Path.GetDirectoryName(m_path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                OptionsToIniData(Osd, nameof(Osd));
                OptionsToIniData(Updater, nameof(Updater));
                OptionsToIniData(AltOsdOptionsFullscreenApps, nameof(AltOsdOptionsFullscreenApps));
                await using (StreamWriter sw = new(m_path))
                {
                    await sw.WriteAsync(m_data.ToString());
                }

                result = true;
            }
            catch { }

            IsWatcherPaused = false;
            return result;
        }

        public static async Task<bool> TryReadAsync()
        {
            bool result = false;
            IsWatcherPaused = false;

            try
            {
                const long MB = 1024 * 1024;
                if (new FileInfo(m_path).Length > 100 * MB) throw new InvalidOperationException("Config file size is too large");

                using StreamReader sr = new(m_path);
                string fileData = await sr.ReadToEndAsync();

                m_data = m_parser.Parse(fileData);
                IniDataToOptions(Osd, nameof(Osd));
                IniDataToOptions(Updater, nameof(Updater));
                IniDataToOptions(AltOsdOptionsFullscreenApps, nameof(AltOsdOptionsFullscreenApps));

                m_data = new();
                result = true;
            }
            catch { }

            IsWatcherPaused = true;
            return result;
        }

        private static void OptionsToIniData(OptionsBase opt, string sectionName)
        {
            foreach (var item in opt.ToDict())
            {
                var optName = item.Key;
                m_data[sectionName][optName] = item.Value;

                var description = opt.GetOptionDescription(optName);
                description.ForEach(str => str = " " + str);
                if (description.Count > 0)
                {
                    m_data[sectionName].GetKeyData(optName).Comments = description;
                }
            }
        }

        private static void IniDataToOptions(OptionsBase opt, string sectionName)
        {
            Dictionary<string, string> dict = new();
            foreach (var item in m_data[sectionName])
            {
                dict.Add(item.KeyName, item.Value);
            }
            opt.FromDict(dict);
        }

        private static async Task ValidateConfigFileAsync()
        {
            _ = await TryReadAsync();
            _ = await TrySaveAsync();
            m_isInit = true;
        }

        private static void Exit()
        {
            m_timer?.Stop();
        }

        private static async void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            m_timer.Start();
            if (await m_timer.WaitForNextTickAsync())
            {
                m_timer.Stop();
                await ValidateConfigFileAsync();
            }
        }
    }
}
