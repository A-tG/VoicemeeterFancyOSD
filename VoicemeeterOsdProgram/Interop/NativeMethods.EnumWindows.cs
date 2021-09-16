using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Interop
{
    public static partial class NativeMethods
    {
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hwnd, StringBuilder lptrString, int nMaxCount);

        public static string GetWindowText(IntPtr hWnd)
        {
            int nRet;
            StringBuilder text = new(256);
            nRet = GetWindowText(hWnd, text, text.Capacity);
            if (nRet != 0)
            {
                return text.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
