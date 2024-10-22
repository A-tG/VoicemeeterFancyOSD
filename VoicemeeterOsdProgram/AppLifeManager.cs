using AtgDev.Utils.ProcessExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VoicemeeterOsdProgram;

public static class AppLifeManager
{
    private static Mutex m_mutex = new(true, Program.UniqueName);
    private static bool? m_isLareadyRunning;
    private static Dispatcher m_dispatcher;
    private static Channel<string[]> m_argsChannel = Channel.CreateUnbounded<string[]>(new() {
        SingleReader = true,
        SingleWriter = true
    });

    public static string[] appArgs = Array.Empty<string>();

    public static bool IsAlreadyRunning
    {
        get
        {
            m_isLareadyRunning ??= !m_mutex.WaitOne(0, false);
            return m_isLareadyRunning.Value;
        }
    }

    public static void Start(string[] args, Action action)
    {
        if (IsAlreadyRunning)
        {
            SendArgsToFirstInstance(args);
            Environment.Exit(0);
        }
        appArgs = args;
        m_dispatcher = Dispatcher.CurrentDispatcher;

        action();
    }

    public static void CloseDuplicates()
    {
        DirectoryInfo programDir = new(AppDomain.CurrentDomain.BaseDirectory);
        // iterating over all exe files because program can be launched by multiple executables
        foreach (var exeFile in programDir.GetFiles("*.exe"))
        {
            string programName = Path.GetFileNameWithoutExtension(exeFile.Name);
            var procs = Process.GetProcessesByName(programName);
            RequestKillDuplicateProcesses(procs);
        }
    }

    public static void StartArgsPipeServer()
    {
        Task.Run(async () => await ProcessArgsLoop());

        Thread pipeServerThread = new(CreatePipeServer)
        {
            IsBackground = true,
        };
        pipeServerThread.SetApartmentState(ApartmentState.STA);
        pipeServerThread.Start();
    }

    private static async ValueTask ProcessArgsLoop()
    {
        await foreach (var a in m_argsChannel.Reader.ReadAllAsync())
        {
            await m_dispatcher.Invoke(async () => await ArgsHandler.HandleAsync(a));
        }
    }

    private static void SendArgsToFirstInstance(string[] args)
    {
        if (args.Length == 0) return;

        using NamedPipeClientStream client = new(".", Program.UniqueName, PipeDirection.Out); // "." is for Local Computer
        try
        {
            client.Connect(1000);
            using StreamWriter writer = new(client);
            foreach (var arg in args)
            {
                writer.WriteLine(arg);
            }
        }
        catch { }
    }

    private static void RequestKillDuplicateProcesses(Process[] procs)
    {
        foreach (var p in procs)
        {
            if (Environment.ProcessId == p.Id) continue;

            p.RequestKill();
            p.WaitForExit(1000);
        }
    }

    private static void CreatePipeServer()
    {
        using NamedPipeServerStream server = new(Program.UniqueName, PipeDirection.In);
        using StreamReader reader = new(server);
        while (true)
        {
            server.WaitForConnection();
            try
            {
                List<string> args = new();
                string arg;
                while (!string.IsNullOrEmpty(arg = reader.ReadLine()))
                {
                    args.Add(arg);
                }
                m_argsChannel.Writer.TryWrite([..args]);
            }
            catch { }
            server.Disconnect();
        }
    }
}
