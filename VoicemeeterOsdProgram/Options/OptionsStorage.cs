using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;

namespace VoicemeeterOsdProgram.Options
{
    public static class OptionsStorage
    {
        // field name will be used as INI file Section name
        public static readonly OsdOptions Osd = new();

        private static readonly string m_path = @$"{AppDomain.CurrentDomain.BaseDirectory}config\config.ini";

        private static readonly FileIniDataParser m_parser = new();
        private static IniData m_data = new();
        private static readonly FileSystemWatcher m_watcher;
        private static DispatcherTimer m_timer = new() { Interval = TimeSpan.FromMilliseconds(1000)};

        static OptionsStorage()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            System.Windows.Application.Current.Exit += (_, _) => Exit();

            TryRead();
            TrySave();

            m_timer.Tick += OnTimerTick;

            m_watcher = new()
            {
                Path = Path.GetDirectoryName(m_path),
                Filter = Path.GetFileName(m_path),
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            m_watcher.Changed += OnConfigFileChanged;
        }

        public static void Init() { }

        public static bool TrySave()
        {
            bool result = false;
            try
            {
                m_data = new();

                var directoryPath = Path.GetDirectoryName(m_path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                ToIniData(Osd);
                m_parser.WriteFile(m_path, m_data);

                result = true;
            }
            catch { }

            return result;
        }

        public static bool TryRead()
        {
            bool result = false;
            try
            {
                m_data = m_parser.ReadFile(m_path);
                FromIniData(Osd);

                result = true;
            }
            catch { }

            return result;
        }

        private static void ToIniData<T>(T optionsObj)
        {
            var sectionName = GetSectionName(optionsObj);
            var properties = optionsObj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                m_data[sectionName][prop.Name] = prop.GetValue(optionsObj).ToString();
            }
        }

        private static void FromIniData<T>(T optionsObj)
        {
            var properties = optionsObj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                ValidateOption(optionsObj, prop);
            }
        }

        private static void ValidateOption<T>(T optionsObj, PropertyInfo optionProp)
        {
            var sectionName = GetSectionName(optionsObj);
            var name = optionProp.Name;
            var field = m_data[sectionName][name];
            try
            {
                if (!string.IsNullOrEmpty(field))
                {
                    var result = Convert.ChangeType(field, optionProp.PropertyType);
                    if (result is not null)
                    {
                        optionProp.SetValue(optionsObj, result);
                        return;
                    }
                }
            }
            catch { }
            m_data[sectionName][name] = optionProp.GetValue(optionsObj).ToString();
        }

        private static string GetSectionName<T>(T optionsObj)
        {
            var members = typeof(OptionsStorage).GetFields(BindingFlags.Static | BindingFlags.Public);
            return members.First(m => m.FieldType.Name == optionsObj.GetType().Name).Name;
        }

        private static void Exit()
        {
            m_timer?.Stop();
        }

        private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            m_timer.Stop();
            m_timer.Start();
            Debug.WriteLine($"config file changed");
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            m_timer.Stop();
            m_watcher.EnableRaisingEvents = false;
            TryRead();
            TrySave();
            m_watcher.EnableRaisingEvents = true;
        }
    }
}
