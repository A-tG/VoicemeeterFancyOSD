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

        private static Timer m_timer;
        private static VoicemeeterType m_type;

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
            get;
            set;
        } = true;

        public static VoicemeeterType ProgramType
        {
            get
            {
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

        public static async void Load()
        {
            if (IsLoaded) return;

            try
            {
                Api = new(PathHelper.GetDllPath());

                Api.Login();
                _ = await Api.WaitForNewParamsAsync(250, 1000 / 30);

                m_timer = new Timer()
                {
                    AutoReset = true,
                    Interval = FastTickTimeMs
                };
                m_timer.Elapsed += OnTimerTick;
                m_timer.Start();

                IsInitialized = IsLoaded = true;
            }
            catch
            {
                if (m_timer is not null)
                {
                    m_timer.Stop();
                    m_timer.Elapsed -= OnTimerTick;
                    m_timer = null;
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
            int res = Api.IsParametersDirty();
            var tickTime = NormalTickTime;

            if (!IsHandlingParams)
            {
                TickTime = SlowTickTimeMs;
                return;
            }

            var type = ProgramType;
            if ((type != m_type) && (type != VoicemeeterType.None))
            {
                m_type = type;
                OnProgramTypeChange(m_type);
            }
            
            switch (res)
            {
                case 0:
                    TickTime = tickTime;
                    break;
                case 1:
                    OnNewParameters();
                    TickTime = tickTime;
                    break;
                default:
                    TickTime = SlowTickTimeMs;
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
