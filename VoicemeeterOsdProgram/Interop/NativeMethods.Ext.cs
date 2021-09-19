using System;
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
    }
}
