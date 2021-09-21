using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace VoicemeeterOsdProgram.Core
{
    public static class VoicemeeterApiClient
    {
        private static DispatcherTimer m_InitTimer;
        private static Stopwatch m_InitStopwatch;
        private const int MaxInitTimeMs = 500;

        public static void Init()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Logout();
            App.Current.Exit += (_, _) => Logout();
            Load();
            if (IsLoaded)
            {
                m_InitTimer = new();
                m_InitTimer.Interval = TimeSpan.FromMilliseconds(1000 / 30);
                m_InitTimer.Tick += OnInitTimer_Tick;
                m_InitStopwatch = new();

                Api.Login();
                m_InitStopwatch.Start();
                m_InitTimer.Start();
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
            Api?.Logout();
        }

        // workaround to "clean" IsParametersDirty()
        // right after Login() it can incorrectly return 1 (New parameters) and it will trigger OSD to show
        // so we call IsParametersDirty() in a "loop" until it returns !0 or MaxInitTimeMs is reached
        private static void OnInitTimer_Tick(object sender, EventArgs e)
        {
            if ((Api.IsParametersDirty() != 0) || (m_InitStopwatch.ElapsedMilliseconds > MaxInitTimeMs))
            {
                m_InitStopwatch.Stop();
                m_InitStopwatch.Stop();
                m_InitTimer.Tick -= OnInitTimer_Tick;
                m_InitTimer = null;
                m_InitStopwatch = null;

                IsInitialized = true;
            }
        }
    }
}
