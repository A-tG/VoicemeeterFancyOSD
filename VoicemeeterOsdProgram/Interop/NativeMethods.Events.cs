namespace TopmostApp.Interop
{
    public static partial class NativeMethods
    {
        public enum Events : uint
        {
            EVENT_SYSTEM_FOREGROUND = 0x0003,
            EVENT_SYSTEM_MINIMIZEEND = 0x0017
        }

        public enum WinEvents : uint
        {
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNTHREAD = 0x0001,
            WINEVENT_SKIPOWNPROCESS = 0x0002,
            WINEVENT_INCONTEXT = 0x0004
        }
    }
}
