﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VoicemeeterOsdProgram.Interop
{
    public static partial class NativeMethods
    {
        public static bool IsBandWindowSupported()
        {
            bool result = NativeLibrary.TryLoad("user32.dll", out IntPtr libHandle);
            if (result)
            {
                result = NativeLibrary.TryGetExport(libHandle, "CreateWindowInBand", out IntPtr procHandle);
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
                using (var proc = Process.GetCurrentProcess())
                {
                    var isImmersive = IsImmersiveProcess(proc.Handle);
                    var hasUiAccess = HasUiAccessProcess(proc.Handle);
                    zbid = isImmersive ? ZBandID.AboveLockUX : hasUiAccess ? ZBandID.UIAccess : ZBandID.Desktop;
                }
            }
            catch { }
            return zbid;
        }
    }
}
