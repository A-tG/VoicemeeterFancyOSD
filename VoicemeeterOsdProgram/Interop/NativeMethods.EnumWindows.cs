using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TopmostApp.Interop
{
    public static partial class NativeMethods
    {
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
}
