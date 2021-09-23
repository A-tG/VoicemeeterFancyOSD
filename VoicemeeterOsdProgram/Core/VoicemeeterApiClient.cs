using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Core
{
    public static class VoicemeeterApiClient
    {

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
    }
}
