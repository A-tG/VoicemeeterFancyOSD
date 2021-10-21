using System;

namespace VoicemeeterOsdProgram.Core
{
    public static class TrayIconManager
    {
        private static UiControls.Tray.TrayIcon m_trayIcon;

        static TrayIconManager()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, _) => m_trayIcon?.NotifyIcon?.Dispose();

            m_trayIcon = new();
        }

        public static void Init() { }
    }
}
