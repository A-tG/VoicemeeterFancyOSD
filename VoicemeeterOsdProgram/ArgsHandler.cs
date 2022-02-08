using AtgDev.Utils.ProcessExtensions;
using System;
using System.Diagnostics;
using System.IO;
using VoicemeeterOsdProgram.Updater;

namespace VoicemeeterOsdProgram
{
    public static class ArgsHandler
    {
        public const string AfterUpdateArg = "--after-update";

        public static void Handle(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0].ToLower() == AfterUpdateArg.ToLower())
                {
                    KillDuplicates();
                    UpdateManager.TryDeleteBackup();
                }
            }
        }

        private static void KillDuplicates()
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

        private static void RequestKillDuplicateProcesses(Process[] procs)
        {
            foreach (var p in procs)
            {
                if (Environment.ProcessId == p.Id) continue;

                p.RequestKill();
                p.WaitForExit(1000);
            }
        }
    }
}
