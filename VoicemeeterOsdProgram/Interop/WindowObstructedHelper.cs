using System;
using System.Collections.Generic;
using System.Linq;
using static TopmostApp.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Interop
{
    public static class WindowObstructedHelper
    {
        private static IntPtr m_targetHwnd;
        private static List<IntPtr> m_windowsOnTop = new();

        public static bool IsObstructed(IntPtr hWnd)
        {
            if ((hWnd == IntPtr.Zero) || !IsWindowVisible(hWnd)) return true;

            m_targetHwnd = hWnd;
            _ = EnumWindows(EnumWindowsHigherZOrder, IntPtr.Zero);

            GetDwmWindowRect(m_targetHwnd, out RECT r);

            bool isInsideScreen = IsRectInScreen(r);
            bool result = !isInsideScreen;

            if (isInsideScreen)
            {
                var targetRect = r.ToRect();
                result = m_windowsOnTop.Any(hWnd =>
                {
                    GetDwmWindowRect(hWnd, out RECT r);
                    return targetRect.IntersectsWith(r.ToRect());
                });
            }

            m_windowsOnTop.Clear();
            m_targetHwnd = IntPtr.Zero;
            return result;
        }

        private static bool IsRectInScreen(RECT r)
        {
            POINTSTRUCT lt = new(r.Left, r.Top);
            POINTSTRUCT rt = new(r.Right, r.Top);
            POINTSTRUCT rb = new(r.Right, r.Bottom);
            POINTSTRUCT lb = new(r.Left, r.Bottom);
            return (MonitorFromPoint(lt) != IntPtr.Zero) && 
                (MonitorFromPoint(rt) != IntPtr.Zero) &&
                (MonitorFromPoint(rb) != IntPtr.Zero) && 
                (MonitorFromPoint(lb) != IntPtr.Zero);
        }

        private static bool EnumWindowsHigherZOrder(IntPtr hWnd, IntPtr lParam)
        {
            if (hWnd == m_targetHwnd) return false;

            if (!IsWindowVisible(hWnd)) return true;

            // detects if Start, Search, Action center, GameBar, MicrosoftStore App windows are visible
            // in case if they are visible for EnumWindows procedure
            if (!IsDwmWindowCloaked(hWnd))
            {
                m_windowsOnTop.Add(hWnd);
            }
            return true;
        }
    }
}
