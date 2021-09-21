using AtgDev.Voicemeeter;
using AtgDev.Voicemeeter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoicemeeterOsdProgram.Core
{
    public static class VoicemeeterApiClient
    {
        public static void Init()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Logout();
            App.Current.Exit += (_, _) => Logout();
            Load();
            if (IsLoaded)
            {
                Api.Login();
                Api.IsParametersDirty();
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
    }
}
