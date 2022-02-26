using AtgDev.Utils.ProcessExtensions;
using System;
using System.Diagnostics;
using System.IO;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.Updater;

namespace VoicemeeterOsdProgram
{
    public static class ArgsHandler
    {
        public static class Args
        {
            public const string AfterUpdateArg = "-after-update";
            public const string Pause = "-pause";
            public const string Unpause = "-unpause";
            public const string TogglePause = "-toggle-pause";
        }

        public static void HandleSpecial(string[] args)
        {
            if ((args.Length == 0) || (args.Length > 1)) return;

            if (args[0].ToLower() == Args.AfterUpdateArg.ToLower())
            {
                AppLifeManager.CloseDuplicates();
                UpdateManager.TryDeleteBackup();
            }
        }

        public static void Handle(string[] args)
        {
            if ((args.Length == 0) || (args.Length > 1)) return;

            var arg = args[0];
            switch (arg)
            {
                case Args.Pause:
                    OptionsStorage.Other.Paused = true;
                    break;
                case Args.Unpause:
                    OptionsStorage.Other.Paused = false;
                    break;
                case Args.TogglePause:
                    OptionsStorage.Other.Paused ^= true; // invert bool
                    break;
                default:
                    break;
            }
        }

        public static void Handle(string rawArgs)
        {
            Debug.WriteLine(rawArgs);
        }
    }
}
