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
                }
            }
        }

        public static void Handle(string[] args)
        {
            var len = args.Length;
            if (len == 0) return;

            for (int i = 0; i < len; i++)
            {
                // handle only first valid argument
                if (HandleArg(args, i)) break;
            }
        }

        private static bool HandleArg(string[] args, int i)
        {
            var arg = args[i].ToLower();
            switch (arg.ToLower())
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
                case Args.SetOption:
                    return SetOption(args, i);
                default:
                    return false;
            }
        }

        private static bool SetOption(string[] args, int i)
        {
            if ((i + 3) >= args.Length) return false;

            var category = args[++i];
            var option = args[++i];
            var val = args[++i];
            switch (category.ToLower())
            {
                case "osd":
                    if (!OptionsStorage.Osd.TryParseFrom(option, val)) return false;
                    break;
                case "altosd":
                    if (!OptionsStorage.AltOsdOptionsFullscreenApps.TryParseFrom(option, val)) return false;
                    break;
                case "updater":
                    if (!OptionsStorage.Updater.TryParseFrom(option, val)) return false;
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
