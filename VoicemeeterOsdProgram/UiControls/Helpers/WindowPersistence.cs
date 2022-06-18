using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace VoicemeeterOsdProgram.UiControls.Helpers
{
    public class WindowPersistence
    {
        private Window m_window;
        private bool m_isWriting = false;
        private FileStream m_fs;
        private string m_winSettingPath = "";

        public AtgDev.Utils.Logger logger;

        public WindowPersistence(Window w, string filePathSaveTo)
        {
            m_window = w;
            m_winSettingPath = filePathSaveTo;
        }

        public async Task<bool> TryReadWindowSettings()
        {
            try
            {
                await ReadWindowSettings();
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error reading Window size {e}");
            }
            return false;
        }

        public async Task<bool> TrySaveWindowSettings()
        {
            bool result = false;
            try
            {
                if (m_isWriting) return false;

                m_isWriting = true;
                await WriteWindowSettings();
                result = true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Error saving Window size {e}");
            }
            finally
            {
                m_isWriting = false;
            }
            return result;
        }

        public void Close()
        {
            m_fs?.Dispose();
            m_fs = null;
        }

        private async Task ReadWindowSettings()
        {
            var c = CultureInfo.InvariantCulture;
            if (!File.Exists(m_winSettingPath)) return;

            // WindowState
            // Left
            // Top
            // Width
            // Height
            using StreamReader sr = new(m_winSettingPath);
            var state = (WindowState)int.Parse(await sr.ReadLineAsync(), c);
            m_window.Left = double.Parse(await sr.ReadLineAsync(), c);
            m_window.Top = double.Parse(await sr.ReadLineAsync(), c);
            if (state == WindowState.Maximized)
            {
                m_window.WindowState = state;
                return;
            }
            m_window.Width = double.Parse(await sr.ReadLineAsync(), c);
            m_window.Height = double.Parse(await sr.ReadLineAsync(), c);
        }

        private async Task WriteWindowSettings()
        {
            var c = CultureInfo.InvariantCulture;

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
            using StreamWriter sw = new(m_fs, leaveOpen: true)
            {
                AutoFlush = false
            };
            var state = m_window.WindowState switch
            {
                WindowState.Normal => 0,
                WindowState.Maximized => 1,
                _ => 0
            };
            await sw.WriteLineAsync(Convert.ToString(state, c));
            await sw.WriteLineAsync(Convert.ToString(m_window.Left, c));
            await sw.WriteLineAsync(Convert.ToString(m_window.Top, c));
            if (m_window.WindowState != WindowState.Maximized)
            {
                await sw.WriteLineAsync(Convert.ToString(m_window.Width, c));
                await sw.WriteLineAsync(Convert.ToString(m_window.Height, c));
            }

            await sw.FlushAsync();
        }
    }
}
