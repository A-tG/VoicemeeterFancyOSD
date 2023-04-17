using AtgDev.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using VoicemeeterOsdProgram.Factories;
using VoicemeeterOsdProgram.Options;
using VoicemeeterOsdProgram.UiControls;
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
            public const string Exit = "-exit";
            public const string Help = "-help";
        }

        private static Dialog m_helpDialog;
        private static Logger m_logger = Globals.Logger;

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
                    break;
                case Args.Unpause:
                    OptionsStorage.Other.Paused = false;
                    break;
                case Args.TogglePause:
                    OptionsStorage.Other.Paused ^= true; // invert bool
                    break;
                case Args.SetOption:
                    return await SetOptionAsync(args, i);
                case Args.Exit:
                    Exit();
                    break;
                case Args.Help:
                    ShowHelpWindow();
                    return true;
                default:
                    m_logger?.LogError($"Unknown command line argument: {arg}");
                    return false;
            }
            m_logger?.Log($"Command line argument processed: {arg}");
            return true;
        }

        private static void Exit()
        {
            var disp = Program.wpf_app?.Dispatcher;
            if (disp is null)
            {
                Environment.Exit(0);
            }
            else
            {
                disp.Invoke(() =>
                {
                    Program.wpf_app.Shutdown();
                });
            }
        }


        private static void ShowHelpWindow()
        {
            const string Msg = 
                "Available commandline arguments:\n" +
                $"{Args.Help}: list available commandline arguments\n" +
                $"{Args.Pause}: stop displaying OSD on Parameters' change\n" +
                $"{Args.Unpause}: resume displaying OSD on Parameters' change\n" +
                $"{Args.TogglePause}: stop/resume displaying OSD\n" +
                $"{Args.SetOption}: Change program options.\n" +
                "\tUsage -set-option category Option value [saveToConfig]\n" +
                "\tcategory: [Category] from config file\n" +
                "\tOption: case sensetive option name under specified category\n" +
                "\tvalue: value to set option to\n" +
                "\tsaveToConfig: (optional) save changes to config file\n" +
                "Examples:\n" +
                "\t-set-option osd BackgroundOpacity 0.3 true\n" +
                "\t-set-option osd IgnoreStripsIndexes \"1, 5\"\n" +
                "\t-set-option osd IgnoreStripsIndexes \" \"";

            if (m_helpDialog is null)
            {
                m_helpDialog = MsgBoxFactory.GetInfo("Help");
                m_helpDialog.ContentToDisplay.Content = new TextBox()
                {
                    IsReadOnly = true,
                    Text = Msg
                };
                m_helpDialog.Closing += (_, _) => m_helpDialog = null;
                m_helpDialog.Show();
            }
            else
            {
                m_helpDialog.Show();
                m_helpDialog.Focus();
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
