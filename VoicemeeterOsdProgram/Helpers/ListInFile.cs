using AtgDev.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Atg.Utils;

public class ListInFile : IEnumerable<string>
{
    private List<string> m_list = new();
    private FileSystemWatcher m_watcher;
    private PeriodicTimerExt m_timer = new(TimeSpan.FromSeconds(1));
    private Dispatcher m_disp = Dispatcher.CurrentDispatcher;

    public ListInFile(string filePath)
    {
        FilePath = filePath;
        _ = Init();
    }

   private async Task Init()
    {
        await TryReadCreateFileAsync();
        m_watcher = new()
        {
            Path = Path.GetDirectoryName(FilePath),
            Filter = Path.GetFileName(FilePath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
        };
        m_watcher.Changed += OnFileChange;
        m_watcher.EnableRaisingEvents = true;
    }

    public string FilePath { get; private set; }

    public bool AllowDuplicates { get; set; } = true;

    public bool IsCaseSensetive { get; set; } = true;

    public async Task<bool> TryWriteAsync()
    {
        bool result = false;
        m_watcher.EnableRaisingEvents = false;
        try
        {
            await WriteAsync();
            result = true;
        }
        catch { }
        m_watcher.EnableRaisingEvents = true;
        return result;
    }

    public async Task<bool> TryReadAsync()
    {
        bool result = false;
        if (!File.Exists(FilePath)) return result;

        Clear();
        try
        {
            await ReadAsync();
            result = true;
        }
        catch { }
        return result;
    }

    public async Task<bool> TryReadCreateFileAsync()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                File.Create(FilePath);
            }
        }
        catch { }
        return await TryReadAsync();
    }

    public void Remove(string item) => m_list.Remove(item);

    public void Clear() => m_list.Clear();

    private async Task WriteAsync()
    {
        using StreamWriter sw = File.CreateText(FilePath);
        foreach (var item in m_list)
        {
            await sw.WriteLineAsync(item);
        }
    }

    private async Task ReadAsync()
    {
        using StreamReader sr = File.OpenText(FilePath);
        string line = "";

        while ((line = await sr.ReadLineAsync()) is not null)
        {
            if (!string.IsNullOrEmpty(line))
            {
                Add(line);
            }
        }
    }

    public void Add(string element)
    {
        if (string.IsNullOrEmpty(element) || string.IsNullOrWhiteSpace(element)) return;

        if (AllowDuplicates)
        {
            m_list.Add(element);
        }
        else
        {
            bool hasElement = m_list.Any(
                el => IsCaseSensetive ? 
                element == el : 
                element.ToLower() == el.ToLower());
            if (!hasElement)
            {
                m_list.Add(element);
            }
        }
    }

    private async void OnFileChange(object sender, FileSystemEventArgs e)
    {
        await m_disp.Invoke(async () =>
        {
            m_timer.Start();
            if (await m_timer.WaitForNextTickAsync())
            {
                m_timer.Stop();
                await TryReadAsync();
            }
        });
    }

    public IEnumerator GetEnumerator() => m_list.GetEnumerator();

    IEnumerator<string> IEnumerable<string>.GetEnumerator() => m_list.GetEnumerator();
}
