namespace VoicemeeterOsdProgram.Core;

public static class TrayIconManager
{
    private static UiControls.Tray.TrayIcon TrayIcon { get; }

    static TrayIconManager()
    {
        TrayIcon = new();
    }

    public static void Init() { }

    public static void OpenUpdater() => TrayIcon.OpenUpdater();

    public static void Destroy() => TrayIcon.NotifyIcon?.Dispose();
}
