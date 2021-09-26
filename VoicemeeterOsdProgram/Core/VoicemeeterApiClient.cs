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
        private static VoicemeeterType m_type = 0;

        public static void Init()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Exit();
            App.Current.Exit += (_, _) => Exit();
            Load();
        }

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

        public static bool IsParamsHandlingEnabled
        {
            get => IsInitialized && m_timer.Enabled;
            set
            {
                m_timer.Enabled = value;
            }
        }

        public static VoicemeeterType ProgramType
        {
            get
            {
                var res = Api.GetVoicemeeterType(out VoicemeeterType type);
                if (res != 0)
                {
                    type = 0;
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
                _ = await WaitForNewParamsAsync();

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

        // workaround to "clean" IsParametersDirty()
        // right after Login() it can incorrectly return 1 (New parameters)
        // so we call IsParametersDirty() in a loop until it returns !0 or maxTime is reached (to not stuck in infinite loop)
        private static async Task<int> WaitForNewParamsAsync(double maxTime = 250, double tickTime = 1000 / 60)
        {
            var timeSpan = TimeSpan.FromMilliseconds(tickTime);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int resp;
            while (((resp = Api.IsParametersDirty()) == 0) && (stopwatch.ElapsedMilliseconds <= maxTime))
            {
                await Task.Delay(timeSpan);
            };
            return resp;
        }

        private static void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            int res = Api.IsParametersDirty();
            var tickTime = NormalTickTime;

            var type = ProgramType;
            if (type != m_type)
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
