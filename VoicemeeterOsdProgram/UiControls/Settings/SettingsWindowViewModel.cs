using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class SettingsWindowViewModel : BaseViewModel
    {
        private double m_width, m_height;
        private WindowState m_state;

        private string m_winSettingConf = Path.Combine(OptionsStorage.ConfigFolderFolder, "SettingsWindow");
        private FileStream m_fs;
        private AtgDev.Utils.Logger m_logger = Globals.logger;

        public double Width
        {
            get => m_width;
            set
            {
                m_width = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => m_height;
            set
            {
                m_height = value;
                OnPropertyChanged();
            }
        }

        public WindowState State
        {
            get => m_state;
            set
            {
                m_state = value;
                OnPropertyChanged();
            }
        }


        public async Task TryReadWindowSettings()
        {
            try
            {
                await ReadWindowSettings();
            }
            catch (Exception e)
            {
                m_logger?.LogError($"Error reading {nameof(SettingsWindow)} size {e}");
            }
        }

        public async Task TrySaveWindowSettings()
        {
            try
            {
                await WriteWindowSettings();
            }
            catch (Exception e)
            {
                m_logger?.LogError($"Error saving {nameof(SettingsWindow)} size {e}");
            }
        }

        public void Close()
        {
            m_fs?.Dispose();
            m_fs = null;
        }

        private async Task ReadWindowSettings()
        {
            var c = CultureInfo.InvariantCulture;
            if (!File.Exists(m_winSettingConf)) return;

            using StreamReader sr = new(m_winSettingConf);
            string line;
            if (string.IsNullOrEmpty(line = await sr.ReadLineAsync())) return;

            int isWindowMax = int.Parse(line, c);
            if (isWindowMax == 1)
            {
                State = WindowState.Maximized;
                return;
            }

            if (string.IsNullOrEmpty(line = await sr.ReadLineAsync())) return;
            Width = double.Parse(line, c);

            if (string.IsNullOrEmpty(line = await sr.ReadLineAsync())) return;
            Height = double.Parse(line, c);
        }

        private async Task WriteWindowSettings()
        {
            var c = CultureInfo.InvariantCulture;

            if (m_fs is null)
            {
                m_fs = new(m_winSettingConf, FileMode.OpenOrCreate, FileAccess.Write);
            }

            m_fs.SetLength(0);

            using StreamWriter sw = new(m_fs, leaveOpen: true)
            {
                AutoFlush = false
            };
            int isWindowMax = State == WindowState.Maximized ? 1 : 0;
            await sw.WriteLineAsync(Convert.ToString(isWindowMax, c));
            if (isWindowMax == 1)
            {
                await m_fs.FlushAsync();
                return;
            }

            await sw.WriteLineAsync(Convert.ToString(Width, c));
            await sw.WriteLineAsync(Convert.ToString(Height, c));
            await sw.FlushAsync();
        }
    }
}
