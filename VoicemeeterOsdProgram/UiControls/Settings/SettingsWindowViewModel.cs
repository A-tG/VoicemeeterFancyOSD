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
        private double m_width = 700, m_height = 770, m_top, m_left;
        private WindowState m_state;
        private bool m_isWriting = false;

        private string m_winSettingConf = Path.Combine(OptionsStorage.ConfigFolder, "SettingsWindow");
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

        public double Top
        {
            get => m_top;
            set
            {
                m_top = value;
                OnPropertyChanged();
            }
        }

        public double Left
        {
            get => m_left;
            set
            {
                m_left = value;
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
                if (m_isWriting) return;

                m_isWriting = true;
                await WriteWindowSettings();
            }
            catch (Exception e)
            {
                m_logger?.LogError($"Error saving {nameof(SettingsWindow)} size {e}");
            }
            finally
            {
                m_isWriting = false;
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

            // WindowState
            // Left
            // Top
            // Width
            // Height
            using StreamReader sr = new(m_winSettingConf);
            var state = (WindowState)int.Parse(await sr.ReadLineAsync(), c);
            Left = double.Parse(await sr.ReadLineAsync(), c);
            Top = double.Parse(await sr.ReadLineAsync(), c);
            if (state == WindowState.Maximized)
            {
                State = state;
                return;
            }
            Width = double.Parse(await sr.ReadLineAsync(), c);
            Height = double.Parse(await sr.ReadLineAsync(), c);
        }

        private async Task WriteWindowSettings()
        {
            var c = CultureInfo.InvariantCulture;

            if (m_fs is null)
            {
                m_fs = new(m_winSettingConf, FileMode.OpenOrCreate, FileAccess.Write);
            }

            m_fs.SetLength(0);

            // WindowState
            // Left
            // Top
            // Width
            // Height
            using StreamWriter sw = new(m_fs, leaveOpen: true)
            {
                AutoFlush = false
            };
            var state = State switch
            {
                WindowState.Normal => 0,
                WindowState.Maximized => 1,
                _ => 0
            };
            await sw.WriteLineAsync(Convert.ToString(state, c));
            await sw.WriteLineAsync(Convert.ToString(Left, c));
            await sw.WriteLineAsync(Convert.ToString(Top, c));
            if (State != WindowState.Maximized)
            {
                await sw.WriteLineAsync(Convert.ToString(Width, c));
                await sw.WriteLineAsync(Convert.ToString(Height, c));
            }

            await sw.FlushAsync();
        }
    }
}
