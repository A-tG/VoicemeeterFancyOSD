using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WpfScreenHelper;
using static TopmostApp.Interop.NativeMethods;

namespace TopmostApp.Helpers
{
    public static class FullscreenAppsWatcher
    {
        public static IEnumerable<string> appsToDetect;

        public static bool IsDetected
        {
            get
            {
                bool result = false;
                try
                {
                    if (appsToDetect is null) return false;

                    IntPtr hWnd = GetForegroundWindow();
                    bool rectRes = GetWindowRect(hWnd, out RECT rect);
                    if (!rectRes) return false;

                    bool isFullscreen = Screen.FromHandle(hWnd).Bounds.Equals(rect.ToRect());
                    if (!isFullscreen) return false;

                    GetWindowThreadProcessId(hWnd, out int ID);
                    var name = Process.GetProcessById(ID).ProcessName;
                    result = appsToDetect.Contains(name);
                }
                catch {}
                return result;
            }
        }
    }
}
