using AtgDev.Utils;
using System.Threading.Tasks;
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

        private static Logger m_logger = Globals.logger;

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

        public static async Task HandleAsync(string[] args)
        {
            var len = args.Length;
            if (len == 0) return;

            for (int i = 0; i < len; i++)
            {
                // handle only first valid argument
                if (await HandleArgAsync(args, i)) break;
            }
        }

        private static async Task<bool> HandleArgAsync(string[] args, int i)
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
                    return await SetOptionAsync(args, i);
                default:
                    m_logger?.LogError($"Unknown command line argument: {arg}");
                    return false;
            }
        }

        private static async Task<bool> SetOptionAsync(string[] args, int i)
        {
            var len = args.Length;
            if ((i + 3) >= len) return false;

            string category = args[++i];
            string option = args[++i];
            string val = args[++i];
            bool isSaveToConfig = false;
            if (++i < len)
            {
                if (!bool.TryParse(args[i], out isSaveToConfig))
                {
                    m_logger?.LogError($"{Args.SetOption} error parsing 4th sub-argument 'saveToConfig'");
                }
            }

            if (OptionsStorage.TryGetSectionOptions(category, out OptionsBase options))
            {
                if (!options.TryParseFrom(option, val))
                {
                    m_logger?.LogError($"{Args.SetOption} error parsing 2nd sub-argument 'CaseSensitiveOptionName'");
                    return false;
                }
            }
            else
            {
                m_logger?.LogError($"{Args.SetOption} error parsing 1st sub-argument 'category'");
                return false;
            }

            if (isSaveToConfig)
            {
                await OptionsStorage.TrySaveAsync();
            }

            return true;
        }
    }
}
