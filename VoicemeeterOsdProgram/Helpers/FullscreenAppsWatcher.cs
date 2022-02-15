using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VoicemeeterOsdProgram.Options;
using WpfScreenHelper;
using static TopmostApp.Interop.NativeMethods;

namespace TopmostApp.Helpers
{
    public static class FullscreenAppsWatcher
    {
        public static IEnumerable<string> appsToDetect;

        private static WinEventDelegate m_winEventDel = new(WinEventProc);
        private static IntPtr m_hWinEventHook;
        private static bool m_isDetected = false;

        static FullscreenAppsWatcher()
        {
            CheckWindow(GetForegroundWindow());
            InitHook();
        }

        public static bool IsDetected
        {
            get => m_isDetected;
            private set
            {
                if (m_isDetected == value) return;

                m_isDetected = value;
                IsDetectedChanged?.Invoke(null, value);
            }
        }

        private static void CheckWindow(IntPtr hWnd)
        {
            IsDetected = IsFullscreenWindow(hWnd) && IsAppToDetect(GetProcessNameFromHwnd(hWnd));
        }


        private static void InitHook()
        {
            m_hWinEventHook = SetWinEventHook((uint)Events.EVENT_SYSTEM_FOREGROUND, (uint)Events.EVENT_SYSTEM_MINIMIZEEND, 
                IntPtr.Zero, m_winEventDel, 0, 0, 
                (uint)WinEvents.WINEVENT_OUTOFCONTEXT);
        }

        private static void Unhook()
        {
            if (m_hWinEventHook == IntPtr.Zero) return;

            UnhookWinEvent(m_hWinEventHook);
        }

        private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch (eventType)
            {
                case (uint)Events.EVENT_SYSTEM_FOREGROUND:
                    Debug.WriteLine("EVENT_FOREGROUND");
                    HandleWinEvent(hWnd);
                    break;
                // EVENT_SYSTEM_FOREGROUND is not activated if window is "un-minimized" from the taskbar
                case (uint)Events.EVENT_SYSTEM_MINIMIZEEND:
                    Debug.WriteLine("EVENT_SYSTEM_MINIMIZEEND");
                    HandleWinEvent(hWnd);
                    break;
                default:
                    break;
            }
        }

        private static string GetProcessNameFromHwnd(IntPtr hWnd)
        {
            string name = "";
            if (hWnd == IntPtr.Zero) return name;

            GetWindowThreadProcessId(hWnd, out int ID);
            try
            {
                name = Process.GetProcessById(ID).ProcessName;
            }
            catch { }
            return name;
        }

        private static void HandleWinEvent(IntPtr hWnd)
        {
            if (IsFullscreenWindow(hWnd)) Debug.WriteLine("Fullscreen app detected");
            if (IsAppToDetect(GetProcessNameFromHwnd(hWnd))) Debug.WriteLine("Correct app");
            CheckWindow(hWnd);

        }

        private static bool IsAppToDetect(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }
            else
            {
                return appsToDetect.Any(el => Path.GetFileNameWithoutExtension(el).ToLower() == name.ToLower());
            }
        }

        private static bool IsFullscreenWindow(IntPtr hWnd)
        {
            bool rectRes = GetWindowRect(hWnd, out RECT rect);
            if (!rectRes) return false;

            bool isFullscreen = Screen.FromHandle(hWnd).Bounds.Equals(rect.ToRect());
            return isFullscreen;
        }

        public static EventHandler<bool> IsDetectedChanged;
    }
}
