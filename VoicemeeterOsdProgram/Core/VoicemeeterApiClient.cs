using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Types;
using AtgDev.Voicemeeter.Utils;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace VoicemeeterOsdProgram.Core
{
    public static class VoicemeeterApiClient
    {
        private const double SlowTickTimeMs = 1000;
        private const double NormalTickTime = 1000 / 30;
        private const double FastTickTimeMs = 1000 / 60;

        private static Timer m_timer = new()
        {
            AutoReset = true,
            Interval = NormalTickTime
        };
        private static VoicemeeterType m_type;
        private static bool m_isSlowUpdate;

        static VoicemeeterApiClient()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            Application.Current.Exit += (_, _) => Exit();

            _ = LoadAsync();
        }

        public static void Init() { }

        public static RemoteApiExtender Api { get; private set; }

        public static bool IsLoaded { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static bool IsVoicemeeterLaunched { get; private set; }

        public static bool IsHandlingParams { get; set; } = true;

        public static VoicemeeterType ProgramType
        {
            get
            {
                if (!IsLoaded) return VoicemeeterType.None;

                if (Api.GetVoicemeeterType(out VoicemeeterType type) != 0)
                {
                    type = VoicemeeterType.None;
                }
                return type;
            }
        }

        private static double TickTime
        {
            get => m_timer.Interval;
            set
            {
                if (m_timer.Interval != value)
                {
                    m_timer.Interval = value;
                }
            }
        }

        private static bool IsSlowUpdate
        {
            get => m_isSlowUpdate;
            set
            {
                m_isSlowUpdate = value;
                TickTime = value ? SlowTickTimeMs : NormalTickTime;
            }
        }

        public static async Task LoadAsync()
        {
            if (IsInitialized) return;

            try
            {
                if (!IsLoaded)
                {
                    Api = new(PathHelper.GetDllPath());
                    Api.Login();
                    _ = await Api.WaitForNewParamsAsync(250, 1000 / 30);

                    IsLoaded = true;

                    OnLoad();
                }

                m_timer.Elapsed += OnTimerTick;
                m_timer.Start();

                IsInitialized = true;
            }
            catch
            {
                if (!IsLoaded)
                {
                    Api?.Dispose();
                    Api = null;
                }

                if (m_timer is not null)
                {
                    m_timer.Stop();
                    m_timer.Elapsed -= OnTimerTick;
                }
            }
        }

        public static void Exit()
        {
            m_timer?.Stop();
            Api?.Logout();
        }

        private static void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            HandleServerConnection();

            if (!IsHandlingParams)
            {
                _ = Api.IsParametersDirty();
                IsSlowUpdate = true;
                return;
            }

            HandleProgramType();
            if (IsVoicemeeterLaunched) HandleParameters();
        }

        private static void HandleServerConnection()
        {
            var dummyCommand = "Strip[0].Mute";
            var res = Api?.GetParameter(dummyCommand, out float _);
            if (res == -2)
            {
                IsSlowUpdate = true;
            }
            else if (res == 0)
            {
                IsSlowUpdate = false;
            }
        }

        private static void HandleProgramType()
        {
            var type = ProgramType;

            if (type != m_type)
            {
                m_type = type;
                OnProgramTypeChange(type);
            }
        }

        private static void HandleParameters()
        {
            if (Api.IsParametersDirty() == 1)
            {
                OnNewParameters();
            }
        }

        private static event EventHandler m_loaded;

        public static event EventHandler NewParameters;
        public static event EventHandler<VoicemeeterType> ProgramTypeChange;
        public static event EventHandler Loaded
        {
            add
            {
                if (IsLoaded)
                {
                    value?.Invoke(null, EventArgs.Empty);
                }
                else
                {
                    m_loaded += value;
                }
            }
            remove => m_loaded -= value;
        }

        private static void OnNewParameters()
        {
            NewParameters?.Invoke(null, EventArgs.Empty);
        }

        private static void OnLoad()
        {
            m_loaded?.Invoke(null, EventArgs.Empty);
        }

        private static void OnProgramTypeChange(VoicemeeterType type)
        {
            ProgramTypeChange?.Invoke(null, type);
            IsVoicemeeterLaunched = type != VoicemeeterType.None;
        }
    }
}
