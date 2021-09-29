using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Types;
using AtgDev.Voicemeeter.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

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
        private static bool m_isHandlingParams = true;

        static VoicemeeterApiClient()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            App.Current.Exit += (_, _) => Exit();
            Load();
        }

        public static void Init() { }

        public static RemoteApiExtender Api
        {
            get;
            private set;
        }

        public static bool IsLoaded
        {
            get;
            private set;
        }

        public static bool IsInitialized
        {
            get;
            private set;
        }

        public static bool IsHandlingParams
        {
            get => m_isHandlingParams;
            set
            {
                m_isHandlingParams = value;
                IsSlowUpdate = !value;
            }
        }

        public static VoicemeeterType ProgramType
        {
            get
            {
                if (!IsLoaded) return VoicemeeterType.None;

                var res = Api.GetVoicemeeterType(out VoicemeeterType type);
                if (res != 0)
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

        public static async void Load()
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
                }

                m_timer.Elapsed += OnTimerTick;
                m_timer.Start();

                IsInitialized = true;
            }
            catch
            {
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
            if (!IsHandlingParams)
            {
                _ = Api.IsParametersDirty();
                IsSlowUpdate = true;
                return;
            }

            HandleProgramType();
            HandleParameters();
        }

        private static void HandleProgramType()
        {
            var type = ProgramType;
            if (type != m_type)
            {
                if (type != VoicemeeterType.None)
                {
                    m_type = type;
                    OnProgramTypeChange(m_type);
                }
            }
        }

        private static void HandleParameters()
        {
            int res = Api.IsParametersDirty();
            switch (res)
            {
                case 0:
                    IsSlowUpdate = false;
                    break;
                case 1:
                    OnNewParameters();
                    IsSlowUpdate = false;
                    break;
                default:
                    IsSlowUpdate = true;
                    break;
            }
        }

        public static event EventHandler NewParameters;
        public static event EventHandler<VoicemeeterType> ProgramTypeChange;

        private static void OnNewParameters()
        {
            NewParameters?.Invoke(null, EventArgs.Empty);
        }

        private static void OnProgramTypeChange(VoicemeeterType type)
        {
            ProgramTypeChange?.Invoke(null, type);
        }
    }
}
