using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static TopmostApp.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Interop
{
    public static class WindowObscureHelper
    {
        private const string CoreWindowClass = "Windows.UI.Core.CoreWindow";
        private const string AppFrameWindowClass = "ApplicationFrameWindow";

        private static IntPtr m_targetHwnd;
        private static List<IntPtr> m_windowsOnTop = new();

        // need to add check for screens bounds, (maybe) taskbar and virtual desktops
        public static bool IsWindowObscured(IntPtr hWnd)
        {
            if ((hWnd == IntPtr.Zero) || !IsWindowVisible(hWnd)) return true;

            m_targetHwnd = hWnd;
            _ = EnumWindows(EnumWindowsHigherZOrder, IntPtr.Zero);
            System.Diagnostics.Debug.WriteLine("");

            GetWindowRect(m_targetHwnd, out RECT r);
            var targetRect = r.ToRect();
            bool result = m_windowsOnTop.Any(hWnd =>
            {
                GetWindowRect(hWnd, out RECT r);
                return targetRect.IntersectsWith(r.ToRect());
            });

            m_windowsOnTop.Clear();
            m_targetHwnd = IntPtr.Zero;
            return result;
        }

        private static bool EnumWindowsHigherZOrder(IntPtr hWnd, IntPtr lParam)
        {
            if (hWnd == m_targetHwnd) return false;

            if (!IsWindowVisible(hWnd)) return true;

            var winClass = GetWindowClassName(hWnd);
            // detects if Start, Search, Action center, GameBar, MicrosoftStore App windows are visible
            if ((winClass == AppFrameWindowClass) || (winClass == CoreWindowClass))
            {
                int cloakVal = 0;
                var res = DwmGetWindowAttribute(hWnd, DwmWindowAttribute.CLOAKED, ref cloakVal, Marshal.SizeOf(cloakVal));
                bool isCloaked = (res == 0) && (cloakVal != 0);
                if (isCloaked) return true;
            }
            m_windowsOnTop.Add(hWnd);
            return true;
        }
    }
}
