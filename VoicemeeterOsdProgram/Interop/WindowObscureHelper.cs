using System;
using System.Collections.Generic;
using System.Linq;
using static TopmostApp.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Interop
{
    public static class WindowObscureHelper
    {
        private static IntPtr targetHwnd;
        private static List<IntPtr> m_windowsOnTop = new();

        public static bool IsWindowObscured(IntPtr hWnd)
        {
            if ((hWnd == IntPtr.Zero) || !IsWindowVisible(hWnd)) return true;

            targetHwnd = hWnd;
            EnumWindows(EnumWindowsHigherZOrder, IntPtr.Zero);

            GetWindowRect(targetHwnd, out RECT r);
            var targetRect = r.ToRect();
            bool result = m_windowsOnTop.Any(hWnd =>
            {
                GetWindowRect(hWnd, out RECT r);
                return targetRect.IntersectsWith(r.ToRect());
            });

            m_windowsOnTop.Clear();
            targetHwnd = IntPtr.Zero;
            return result;
        }

        private static bool EnumWindowsHigherZOrder(IntPtr hWnd, IntPtr lParam)
        {
            bool isFound = hWnd == targetHwnd;
            if (isFound)
            {
                targetHwnd = hWnd;
            }
            else
            {
                if (IsWindowVisible(hWnd))
                {
                    m_windowsOnTop.Add(hWnd);
                }
            }
            return !isFound;
        }
    }
}
