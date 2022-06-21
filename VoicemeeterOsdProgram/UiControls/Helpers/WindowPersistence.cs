using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VoicemeeterOsdProgram.UiControls.Helpers
{
    public class WindowPersistence : IDisposable
    {
        private readonly Window m_window;
        private FileStream m_fs;
        private readonly string m_winSettingPath;

        private bool m_disposed = false;

        public AtgDev.Utils.Logger logger;

        public WindowPersistence(Window w, string filePathSaveTo)
        {
            m_window = w;
            m_winSettingPath = filePathSaveTo;
        }

        public int MaxFileSize => 512;

        public bool TryReadWindowSettings()
        {
            if (!Monitor.TryEnter(this)) return false;

            try
            {
                ReadWindowSettings();
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error reading Window size {e}");
            }
            Monitor.Exit(this);
            return false;
        }

        public async Task<bool> TryReadWindowSettingsAsync()
        {
            if (!Monitor.TryEnter(this)) return false;

            try
            {
                await ReadWindowSettingsAsync();
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error reading Window size {e}");
            }
            Monitor.Exit(this);
            return false;
        }

        public async Task<bool> TrySaveWindowSettingsAsync()
        {
            bool result = false;
            if (!Monitor.TryEnter(this)) return false;

            try
            {
                await WriteWindowSettingsAsync();
                result = true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error saving Window size {e}");
            }
            Monitor.Exit(this);
            return result;
        }

        public bool TrySaveWindowSettings()
        {
            bool result = false;
            if (!Monitor.TryEnter(this)) return false;

            try
            {
                WriteWindowSettings();
                result = true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error saving Window size {e}");
            }
            Monitor.Exit(this);
            return result;
        }

        private bool IsValidSettingsFile()
        {
            if (!File.Exists(m_winSettingPath)) return false;
            if ((new FileInfo(m_winSettingPath).Length) > MaxFileSize) throw new ArgumentException($"{m_winSettingPath} file is too big");

            return true;
        }

        private async Task ReadWindowSettingsAsync()
        {
            if (!IsValidSettingsFile()) return;

            using StreamReader sr = new(m_winSettingPath);
            using StringReader strR = new(await sr.ReadToEndAsync());
            ReadData(strR);
        }

        private void ReadWindowSettings()
        {
            if (!IsValidSettingsFile()) return;

            using StreamReader sr = new(m_winSettingPath);
            using StringReader strR = new(sr.ReadToEnd());
            ReadData(strR);
        }

        private StreamWriter GetWriteStream()
        {
            m_fs ??= new(m_winSettingPath, FileMode.OpenOrCreate, FileAccess.Write);
            m_fs.SetLength(0);
            return new StreamWriter(m_fs, leaveOpen: true);
        }

        private async Task WriteWindowSettingsAsync()
        {
            await using StreamWriter sw = GetWriteStream();
            await sw.WriteAsync(GetData());
            await sw.FlushAsync();
        }

        private void WriteWindowSettings()
        {
            using StreamWriter sw = GetWriteStream();
            sw.Write(GetData());
            sw.Flush();
        }

        private void ReadData(StringReader data)
        {
            var c = CultureInfo.InvariantCulture;
            // WindowState
            // Left
            // Top
            // Width
            // Height
            var state = (WindowState)int.Parse(data.ReadLine(), c);
            m_window.Left = double.Parse(data.ReadLine(), c);
            m_window.Top = double.Parse(data.ReadLine(), c);
            if (state == WindowState.Maximized)
            {
                m_window.WindowState = state;
                return;
            }
            m_window.Width = double.Parse(data.ReadLine(), c);
            m_window.Height = double.Parse(data.ReadLine(), c);
        }

        private string GetData()
        {
            // WindowState
            // Left
            // Top
            // Width
            // Height
            var state = m_window.WindowState switch
            {
                WindowState.Normal => 0,
                WindowState.Maximized => 1,
                _ => 0
            };
            string data = FormattableString.Invariant($"{state}\n{m_window.Left}\n{m_window.Top}");
            if (m_window.WindowState != WindowState.Maximized)
            {
                data += FormattableString.Invariant($"\n{m_window.ActualWidth}\n{m_window.ActualHeight}");
            }
            return data;
        }

        public void Dispose()
        {
            if (m_disposed) return;

            m_fs?.Dispose();
            m_disposed = true;
        }
    }
}
