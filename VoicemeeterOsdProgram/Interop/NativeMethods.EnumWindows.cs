using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TopmostApp.Interop;

public static partial class NativeMethods
{
    public enum DwmWindowAttribute
    {
        NCRENDERING_ENABLED = 1,
        NCRENDERING_POLICY,
        TRANSITIONS_FORCEDISABLED,
        ALLOW_NCPAINT,
        CAPTION_BUTTON_BOUNDS,
        NONCLIENT_RTL_LAYOUT,
        FORCE_ICONIC_REPRESENTATION,
        FLIP3D_POLICY,
        EXTENDED_FRAME_BOUNDS,
        HAS_ICONIC_BITMAP,
        DISALLOW_PEEK,
        EXCLUDED_FROM_PEEK,
        CLOAK,
        CLOAKED,
        FREEZE_REPRESENTATION,
        LAST
    }

    [DllImport("Dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

    [DllImport("Dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref RECT rect, int cbAttribute);

    /// <summary>
    ///     Relevant to Win 8+ Start, Action center, Search, etc, Microsoft Store apps windows,
    ///     because IsWindowVisible always returns true for these.
    /// </summary>
    public static bool IsDwmWindowCloaked(IntPtr hWnd)
    {
        int cloakVal = 0;
        var res = DwmGetWindowAttribute(hWnd, DwmWindowAttribute.CLOAKED, ref cloakVal, Marshal.SizeOf(cloakVal));
        return (res == 0) && (cloakVal != 0);
    }

    public static int GetDwmWindowRect(IntPtr hWnd, out RECT r)
    {
        r = new RECT();
        var res = DwmGetWindowAttribute(hWnd, DwmWindowAttribute.EXTENDED_FRAME_BOUNDS, ref r, Marshal.SizeOf(r));
        return res;
    }

    public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern int GetWindowText(IntPtr hwnd, StringBuilder lptrString, int nMaxCount);

    public static string GetWindowText(IntPtr hWnd)
    {
        StringBuilder text = new(256);
        var res = GetWindowText(hWnd, text, text.Capacity);
        return (res != 0) ? text.ToString() : string.Empty;
    }
}
