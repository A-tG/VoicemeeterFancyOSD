using AtgDev.Utils.Extensions;
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
                    string programName = Path.GetFileNameWithoutExtension(Environment.ProcessPath);
                    var procs = Process.GetProcessesByName(programName);
                    foreach (var p in procs)
                    {
                        if (Environment.ProcessId == p.Id) continue;

                        ProcessExtensions.RequestKill(p);
                        p.WaitForExit(1000);
                    }
                    UpdateManager.TryDeleteBackup();
                }
            }
        }
    }
}
