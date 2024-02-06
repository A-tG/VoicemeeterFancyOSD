using AtgDev.Utils;
using IniParser.Model;
using IniParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VoicemeeterOsdProgram.Options;

public static class OptionsStorage
{
    public static readonly ProgramOptions Program = new();
    public static readonly OsdOptions Osd = new();
    public static readonly VoicemeeterOptions Voicemeeter = new();
    public static readonly OsdAlternative AltOsdOptionsFullscreenApps = new();
    public static readonly UpdaterOptions Updater = new();
    public static readonly LoggerOption Logger = new();
    public static readonly OtherOptions Other = new();

    private static readonly IniDataParser m_parser = new();
    private static IniData m_data = new();
    private static FileSystemWatcher m_watcher = new();
    private static PeriodicTimerExt m_timer = new(TimeSpan.FromSeconds(1));
    private static bool m_isWatcherEnabled;
    private static bool m_isWatcherPaused;
    private static bool m_isInit = false;
    private static Dispatcher m_disp;
    private static Dictionary<string, OptionsBase> m_sectionsOptions;

    public static string ConfigFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");

    public static string ConfigFilePath { get; } = Path.Combine(ConfigFolder, "config.ini");

    private static Logger m_logger = Globals.Logger;

    static OptionsStorage()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
        System.Windows.Application.Current.Exit += (_, _) => Exit();

        Program.logger = m_logger;
        Osd.logger = m_logger;
        Voicemeeter.logger = m_logger;
        AltOsdOptionsFullscreenApps.logger = m_logger;
        Updater.logger = m_logger;
        Logger.logger = m_logger;
    }


    public static async Task InitAsync()
    {
        if (m_isInit) return;

        m_isInit = true;

        await ValidateConfigFileAsync();

        m_disp = Dispatcher.CurrentDispatcher;
        m_watcher.Path = Path.GetDirectoryName(ConfigFilePath);
        m_watcher.Filter = Path.GetFileName(ConfigFilePath);
        m_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
        m_watcher.Changed += OnConfigFileChanged;
        IsWatcherEnabled = true;
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

    public static IEnumerable<string> Sections => SectionsOptions.Keys;

    private static IDictionary<string, OptionsBase> SectionsOptions
    {
        get
        {
            return m_sectionsOptions ??= typeof(OptionsStorage).
                GetFields(BindingFlags.Public | BindingFlags.Static).
                Where(o =>
                {
                    var val = o.GetValue(null);
                    return (val is OptionsBase) && (val is not OtherOptions);
                }).
                ToDictionary(f => f.Name.ToLower(), f => (OptionsBase)f.GetValue(null));
        }
    }

    public static bool TryGetSectionOptions(string sectionName, out OptionsBase options)
    {
        if (SectionsOptions.TryGetValue(sectionName.ToLower(), out OptionsBase o))
        {
            options = o;
            return true;
        }

        options = default;
        return false;
    }

    public static void SaveData()
    {
        var directoryPath = Path.GetDirectoryName(ConfigFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        OptionsToIniData(Program, nameof(Program));
        OptionsToIniData(Osd, nameof(Osd));
        OptionsToIniData(Voicemeeter, nameof(Voicemeeter));
        OptionsToIniData(Updater, nameof(Updater));
        OptionsToIniData(AltOsdOptionsFullscreenApps, nameof(AltOsdOptionsFullscreenApps));
        OptionsToIniData(Logger, nameof(Logger));
    }

    public static bool TrySave()
    {

        m_logger?.Log("Writing config...");
        bool result = false;
        if (!m_isInit) return result;

        IsWatcherPaused = true;

        try
        {
            SaveData();
            using (StreamWriter sw = new(ConfigFilePath))
            {
                sw.Write(m_data.ToString());
            }

            result = true;
            m_logger?.Log("Writing config: OK");
        }
        catch (Exception e)
        {
            m_logger?.LogError($"Writing config: FAILED {e.GetType} {e.Message}");
        }

        IsWatcherPaused = false;
        return result;
    }

    public static async Task<bool> TrySaveAsync()
    {
        m_logger?.Log("Writing config...");
        bool result = false;
        if (!m_isInit) return result;

        IsWatcherPaused = true;

        try
        {
            SaveData();
            await using (StreamWriter sw = new(ConfigFilePath))
            {
                await sw.WriteAsync(m_data.ToString());
            }

            result = true;
            m_logger?.Log("Writing config: OK");
        }
        catch (Exception e)
        {
            m_logger?.LogError($"Writing config: FAILED {e.GetType} {e.Message}");
        }

        IsWatcherPaused = false;
        return result;
    }

    public static async Task<bool> TryReadAsync()
    {
        m_logger?.Log("Reading config...");
        bool result = false;
        if (!m_isInit) return result;

        IsWatcherPaused = false;

        try
        {
            const long MB = 1024 * 1024;
            if (new FileInfo(ConfigFilePath).Length > 100 * MB) throw new InvalidOperationException("Config file size is too large");

            using StreamReader sr = new(ConfigFilePath);
            string fileData = await sr.ReadToEndAsync();

            m_data = m_parser.Parse(fileData);
            IniDataToOptions(Program, nameof(Program));
            IniDataToOptions(Osd, nameof(Osd));
            IniDataToOptions(Voicemeeter, nameof(Voicemeeter));
            IniDataToOptions(Updater, nameof(Updater));
            IniDataToOptions(AltOsdOptionsFullscreenApps, nameof(AltOsdOptionsFullscreenApps));
            IniDataToOptions(Logger, nameof(Logger));

            m_data = new();
            result = true;
            m_logger?.Log("Reading config: OK");
        }
        catch (Exception e)
        {
            m_logger?.LogError($"Reading config: FAILED {e.GetType} {e.Message}");
        }

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
            for (int i = 0; i < description.Count; i++) // prepend space
            {
                description[i] = " " + description[i];
            }
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
        // need to to use Dispatcher or this code will run on another thread
        _ = await m_disp.InvokeAsync(async () =>
          {
              m_timer.Start();
              if (await m_timer.WaitForNextTickAsync())
              {
                  m_timer.Stop();
                  m_logger?.Log("Config file changed, validating...");
                  await ValidateConfigFileAsync();
              }
          });
    }
}
