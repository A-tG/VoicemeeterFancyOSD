using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TopmostApp.Interop
{
    public static partial class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern int PostThreadMessage(int idThread, uint msg, int wParam, int lParam);

        public static bool IsBandWindowSupported()
        {
            bool result = NativeLibrary.TryLoad("user32.dll", out IntPtr libHandle);
            if (result)
            {
                result = NativeLibrary.TryGetExport(libHandle, "CreateWindowInBand", out _);
                NativeLibrary.Free(libHandle);
            }
            return result;
        }

        public static ZBandID GetTopMostZBandID()
        {
            var zbid = ZBandID.Default;
            // if procedures are not supported in old Windows version
            try
            {
                using var proc = Process.GetCurrentProcess();

                var isImmersive = IsImmersiveProcess(proc.Handle);
                var hasUiAccess = HasUiAccessProcess(proc.Handle);
                bool isWinTenOrNewer = Environment.OSVersion.Version >= new Version(10, 0);

                ZBandID topId = isWinTenOrNewer ? ZBandID.AboveLockUX : ZBandID.SystemTools;
                zbid = isImmersive ? topId : hasUiAccess ? ZBandID.UIAccess : ZBandID.Desktop;
            }
            catch { }
            return zbid;
        }
    }
}
