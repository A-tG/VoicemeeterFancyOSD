using System;
using System.Windows;

namespace VoicemeeterOsdProgram.Core
{
    public static class TrayIconManager
    {
        private static UiControls.Tray.TrayIcon m_trayIcon;

        static TrayIconManager()
        {
            Application.Current.DispatcherUnhandledException += (_, _) => Destroy();
            AppDomain.CurrentDomain.UnhandledException += (_, _) => Destroy();
            m_trayIcon = new();
        }

        public static void Init() { }

        public static void OpenUpdaterWindow()
        {
            m_trayIcon?.CheckForUpdate();
        }

        public static void Destroy()
        {
            m_trayIcon?.NotifyIcon?.Dispose();
        }
    }
}
