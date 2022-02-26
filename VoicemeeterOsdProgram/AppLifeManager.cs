using AtgDev.Utils.ProcessExtensions;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram
{
    public static class AppLifeManager
    {
        private static App m_app;
        private static Mutex m_mutex = new(true, Program.UniqueName);
        private static bool? m_isLareadyRunning;

        public static bool IsAlreadyRunning
        {
            get
            {
                if (m_isLareadyRunning is null)
                {
                    m_isLareadyRunning = !m_mutex.WaitOne(0, false);
                }
                return m_isLareadyRunning.Value;
            }
        }

        public static App Start(string[] args)
        {
            if (IsAlreadyRunning)
            {
                SendArgsToFirstInstance(args);
                Environment.Exit(0);
            }

            ArgsHandler.Handle(args);
            _ = CreatePipeServerAsync();
            m_app = new();
            m_app.Run();
            return m_app;
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

        private static void SendArgsToFirstInstance(string[] args)
        {
            if (args.Length == 0) return;

            var rawArgs = string.Join(' ', args);
            if (string.IsNullOrWhiteSpace(rawArgs)) return;

            using NamedPipeClientStream client = new(".", Program.UniqueName, PipeDirection.Out); // "." is for Local Computer
            try
            {
                client.Connect(1000);
                using StreamWriter writer = new StreamWriter(client);
                writer.Write(rawArgs);
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

        private static async Task CreatePipeServerAsync()
        {
            using NamedPipeServerStream server = new(Program.UniqueName, PipeDirection.In);
            using StreamReader reader = new(server);
            while (true)
            {
                await server.WaitForConnectionAsync();
                try
                {
                    string rawArgs = await reader.ReadToEndAsync();
                    ArgsHandler.Handle(rawArgs);
                }
                catch { }
                server.Disconnect();
            }
        }
    }
}
