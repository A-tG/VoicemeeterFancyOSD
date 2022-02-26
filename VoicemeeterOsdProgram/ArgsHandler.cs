using AtgDev.Utils.ProcessExtensions;
using System;
using System.Diagnostics;
using System.IO;
using VoicemeeterOsdProgram.Updater;

namespace VoicemeeterOsdProgram
{
    public static class ArgsHandler
    {
        public static class Args
        {
            public const string AfterUpdateArg = "--after-update";
        }

        public static void HandleSpecial(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0].ToLower() == Args.AfterUpdateArg.ToLower())
                {
                    AppLifeManager.CloseDuplicates();
                    UpdateManager.TryDeleteBackup();
                }
            }
        }

        public static void Handle(string[] args)
        {

        }

        public static void Handle(string rawArgs)
        {
            Debug.WriteLine(rawArgs);
        }
    }
}
