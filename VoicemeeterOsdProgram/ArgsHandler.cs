using System;
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
            public const string SetOption = "-set-option";
        }

        public static string[] SplitRawArgs(string rawArgs)
        {
            return rawArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);;
        }

        public static void HandleSpecial(string[] args)
        {
            var len = args.Length;
            if (len == 0) return;

            for (int i = 0; i < len; i++)
            {
                var arg = args[i].ToLower();
                if (arg == Args.AfterUpdateArg.ToLower())
                {
                    AppLifeManager.CloseDuplicates();
                    UpdateManager.TryDeleteBackup();
                    break;
                } else if (arg == Args.SetOption.ToLower())
                {
                    // set Option
                    break;
                }
            }
        }

        public static void Handle(string[] args)
        {
            var len = args.Length;
            if (len == 0) return;

            for (int i = 0; i < len; i++)
            {
                if (HandleArg(args[i])) break;
            }
        }

        private static bool HandleArg(string arg)
        {
            switch (arg)
            {
                case Args.Pause:
                    OptionsStorage.Other.Paused = true;
                    return true;
                case Args.Unpause:
                    OptionsStorage.Other.Paused = false;
                    return true;
                case Args.TogglePause:
                    OptionsStorage.Other.Paused ^= true; // invert bool
                    return true;
                default:
                    return false;
            }
        }

        public static void Handle(string rawArgs) => Handle(SplitRawArgs(rawArgs));
    }
}
