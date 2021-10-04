using System;
using System.Collections.Generic;
using System.Linq;
using static TopmostApp.Interop.NativeMethods;

namespace VoicemeeterOsdProgram.Interop
{
    public static class WindowObstructedHelper
    {
        private const string CoreWindowClass = "Windows.UI.Core.CoreWindow";
        private const string AppFrameWindowClass = "ApplicationFrameWindow";

        private static IntPtr m_targetHwnd;
        private static List<IntPtr> m_windowsOnTop = new();

        public static bool IsObstructed(IntPtr hWnd)
        {
            if ((hWnd == IntPtr.Zero) || !IsWindowVisible(hWnd)) return true;

            m_targetHwnd = hWnd;
            _ = EnumWindows(EnumWindowsHigherZOrder, IntPtr.Zero);

            GetWindowRect(m_targetHwnd, out RECT r);

            bool isInsideScreen = IsRectInScreen(r);
            bool result = !isInsideScreen;

            if (isInsideScreen)
            {
                var targetRect = r.ToRect();
                result = m_windowsOnTop.Any(hWnd =>
                {
                    GetWindowRect(hWnd, out RECT r);
                    return targetRect.IntersectsWith(r.ToRect());
                });
            }

            m_windowsOnTop.Clear();
            m_targetHwnd = IntPtr.Zero;
            return result;
        }

        private static bool IsRectInScreen(RECT r)
        {
            POINTSTRUCT topLeft = new(r.Left, r.Top);
            POINTSTRUCT bottomRight = new(r.Right, r.Bottom);
            return (MonitorFromPoint(topLeft, 0) != IntPtr.Zero) && (MonitorFromPoint(bottomRight, 0) != IntPtr.Zero);
        }

        private static bool EnumWindowsHigherZOrder(IntPtr hWnd, IntPtr lParam)
        {
            if (hWnd == m_targetHwnd) return false;

            if (!IsWindowVisible(hWnd)) return true;

            var winClass = GetWindowClassName(hWnd);
            // detects if Start, Search, Action center, GameBar, MicrosoftStore App windows are visible
            if ((winClass == AppFrameWindowClass) || (winClass == CoreWindowClass))
            {
                if (IsWindowCloaked(hWnd)) return true;
            }
            m_windowsOnTop.Add(hWnd);
            return true;
        }
    }
}
