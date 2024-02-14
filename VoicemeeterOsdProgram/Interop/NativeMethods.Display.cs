using System;

namespace TopmostApp.Interop;

public partial class NativeMethods
{
    public enum MONITOR
    {
        DEFAULTTONULL = 0,
        DEFAULTTOPRIMARY,
        DEFAULTTONEAREST,
    }

    internal static IntPtr MonitorFromPoint(POINTSTRUCT pt, MONITOR flag = MONITOR.DEFAULTTONULL)
    {
        return MonitorFromPoint(pt, (int)flag);
    }
}
