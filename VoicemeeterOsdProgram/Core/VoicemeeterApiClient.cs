using AtgDev.Voicemeeter;
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
        private const double FastTickTimeMs = 1000 / 60;

        private static Timer m_timer;

        public static async void Init()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Logout();
            App.Current.Exit += (_, _) => Logout();
            Load();
            if (IsLoaded)
            {
                Api.Login();
                _ = await WaitForNewParamsAsync();
                IsInitialized = true;

                m_timer = new Timer()
                {
                    AutoReset = true,
                    Interval = FastTickTimeMs
                };
                m_timer.Elapsed += OnTimerTick;
                m_timer.Start();
            }
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

        public static void Load()
        {
            if (IsLoaded) return;

            try
            {
                Api = new(PathHelper.GetDllPath());
                IsLoaded = true;
            }
            catch { }
        }

        public static void Logout()
        {
            m_timer.Stop();
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
            switch (res)
            {
                case 1:
                    OnNewParameters();
                    break;
            }
            TickTime = (res == -2) ? SlowTickTimeMs : FastTickTimeMs;
        }

        public static event EventHandler NewParameters;

        private static void OnNewParameters()
        {
            NewParameters?.Invoke(null, EventArgs.Empty);
        }
    }
}
