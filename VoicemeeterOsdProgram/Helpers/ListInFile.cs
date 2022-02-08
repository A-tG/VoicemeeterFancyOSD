using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace Atg.Utils
{
    public class ListInFile : IEnumerable<string>
    {
        private List<string> m_list = new();
        private FileSystemWatcher m_watcher;
        private Timer m_timer = new(1000);

        public string FilePath { get; private set; }

        public ListInFile(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(filePath))
            {
                _ = ReadAsync();
            }
            else
            {
                File.Create(filePath);
            }
            m_watcher = new()
            {
                Path = Path.GetDirectoryName(filePath),
                Filter = Path.GetFileName(filePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            };
            m_watcher.Changed += OnFileChange;
            m_timer.Elapsed += OnTimerTick;
            m_watcher.EnableRaisingEvents = true;
        }

        private async Task<bool> TryReadAsync()
        {
            bool result = false;
            try
            {
                await ReadAsync();
                result = true;
            }
            catch { }
            return result;

        }

        private async Task ReadAsync()
        {
            using StreamReader sr = File.OpenText(FilePath);
            string line = "";

            while ((line = await sr.ReadLineAsync()) is not null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    m_list.Add(Path.GetFileNameWithoutExtension(line));
                }
            }
        }

        private void OnFileChange(object sender, FileSystemEventArgs e)
        {
            m_timer.Stop();
            m_timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            m_timer.Stop();
            _ = TryReadAsync();
        }

        public IEnumerator GetEnumerator() => m_list.GetEnumerator();

        IEnumerator<string> IEnumerable<string>.GetEnumerator() => m_list.GetEnumerator();
    }
}
