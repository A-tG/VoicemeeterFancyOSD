using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AtgDev.Utils;

public class Logger : IDisposable, IAsyncDisposable
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
        Critical,
        Debug
    }

    public class Message
    {
        public Message(string text, LogType t = LogType.Info)
        {
            Text = text;
            Type = t;
            Timestamp = DateTime.Now;
        }

        public LogType Type { get; private set; }

        public string Text { get; private set; }

        public DateTime Timestamp { get; private set; }
    }

    private Channel<Message> m_messageChannel = Channel.CreateUnbounded<Message>(new() {
        SingleReader = true,
        SingleWriter = true
    });
    private CancellationTokenSource m_tokenSource = new();
    private CancellationToken m_token;
    private StreamWriter m_writer;
    private uint m_maxLogs = 0;

    public Logger(string folderPath)
    {
        if (!Directory.Exists(folderPath)) throw new DirectoryNotFoundException(folderPath);

        m_token = m_tokenSource.Token;

        FolderPath = folderPath;

        Task.Run(async () => await Loop());
    }

    public string FolderPath { get; }

    public bool IsEnabled { get; set; } = false;

    public uint MaxLogFiles
    { 
        get => m_maxLogs; 
        set
        {
            if (m_maxLogs == value) return;

            m_maxLogs = value;
            DeleteMaxLogFiles();
        }
    }

    public void Log(Message m)
    {
        if (!IsEnabled) return;

        m_messageChannel.Writer.TryWrite(m);
    }

    public void Log(string text, LogType type = LogType.Info) => Log(new Message(text, type));

    public void LogWarning(string text) => Log(new Message(text, LogType.Warning));

    public void LogError(string text) => Log(new Message(text, LogType.Error));

    public void LogCritical(string text) => Log(new Message(text, LogType.Critical));

    public void LogDebug(string text) => Log(text, LogType.Debug);

    private bool TryCreateLogFile()
    {
        try
        {
            m_writer = new StreamWriter(GenerateLogFileFullName());
            return true;
        }
        catch { }
        return false;
    }

    private void DeleteMaxLogFiles()
    {
        var max = MaxLogFiles;
        if (max == 0) return;

        var files = Directory.GetFiles(FolderPath, "*.log", SearchOption.TopDirectoryOnly);
        if (files.Length <= max) return;

        var oldestFiles = files.OrderBy(f => File.GetLastWriteTime(f)).Take(files.Length - (int)max);
        foreach (var f in oldestFiles)
        {
            try
            {
                File.Delete(f);
            }
            catch { }
        }
    }

    private string GenerateLogFileFullName()
    {
        string result = "";
        string nameWoExt = $"{DateTime.Now:dd-MM-yyyy HH-mm-ss}";
        for (int i = 0; i <= 100; i++)
        {
            string suff = (i == 0) ? "" : $" ({i})";
            string path = Path.Combine(FolderPath, $"{nameWoExt}{suff}.log");
            if (!File.Exists(path))
            {
                result = path;
                break;
            }
        }
        return result;
    }

    private async ValueTask Loop()
    {
        await foreach (var m in m_messageChannel.Reader.ReadAllAsync(m_token))
        {
#if !DEBUG
                if (m.Type == LogType.Debug) continue;
#endif
            TryWrite(m);
        }
    }

    private bool TryWrite(Message m)
    {
        string t = m.Type switch
        {
            LogType.Info => "",
            _ => m.Type.ToString().ToUpper(),
        };
        if (!string.IsNullOrEmpty(t))
        {
            t = t.PadLeft(t.Length + 1);
        }
        return TryWrite($"{m.Timestamp:dd-MM-yyyy HH:mm:ss.fff}{t} {m.Text}");
    }

    private bool TryWrite(string text)
    {
        if (m_writer is null)
        {
            if (!TryCreateLogFile()) return false;
        }

        try
        {
            m_writer.WriteLine(text);
            m_writer.Flush();
            return true;
        }
        catch { }
        return false;
    }

    private bool m_disposed = false;

    public void Dispose()
    {
        if (m_disposed) return;

        m_writer?.Dispose();
        m_messageChannel.Writer.Complete();

        m_disposed = true;

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (m_disposed) return;

        await (m_writer?.DisposeAsync() ?? ValueTask.CompletedTask);
        m_tokenSource.Cancel();
        m_tokenSource.Dispose();
        m_disposed = true;
        
        GC.SuppressFinalize(this);
    }

    ~Logger()
    {
        Dispose();
    }
}
