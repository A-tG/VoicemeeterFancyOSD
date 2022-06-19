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

        public async Task<bool> TryReadWindowSettings()
        {
            if (!Monitor.TryEnter(this)) return false;

            try
            {
                await ReadWindowSettings();
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error reading Window size {e}");
            }
            Monitor.Exit(this);
            return false;
        }

        public async Task<bool> TrySaveWindowSettings()
        {
            bool result = false;
            if (!Monitor.TryEnter(this)) return false;

            try
            {
                await WriteWindowSettings();
                result = true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error saving Window size {e}");
            }
            Monitor.Exit(this);
            return result;
        }

        private async Task ReadWindowSettings()
        {
            var c = CultureInfo.InvariantCulture;
            if (!File.Exists(m_winSettingPath)) return;
            if ((new FileInfo(m_winSettingPath).Length) > MaxFileSize) return;

            // WindowState
            // Left
            // Top
            // Width
            // Height
            using StreamReader sr = new(m_winSettingPath);
            using StringReader strR = new(await sr.ReadToEndAsync());

            var state = (WindowState)int.Parse(strR.ReadLine(), c);
            m_window.Left = double.Parse(strR.ReadLine(), c);
            m_window.Top = double.Parse(strR.ReadLine(), c);
            if (state == WindowState.Maximized)
            {
                m_window.WindowState = state;
                return;
            }
            m_window.Width = double.Parse(strR.ReadLine(), c);
            m_window.Height = double.Parse(strR.ReadLine(), c);
        }

        private async Task WriteWindowSettings()
        {
            if (m_fs is null)
            {
                m_fs = new(m_winSettingPath, FileMode.OpenOrCreate, FileAccess.Write);
            }
            m_fs.SetLength(0);

            // WindowState
            // Left
            // Top
            // Width
            // Height
            await using StreamWriter sw = new(m_fs, leaveOpen: true);
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

            sw.Write(data);
            await sw.FlushAsync();
        }

        public void Dispose()
        {
            if (m_disposed) return;

            m_fs?.Dispose();
            m_disposed = true;
        }
    }
}
