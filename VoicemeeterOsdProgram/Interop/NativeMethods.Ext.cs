using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TopmostApp.Interop;

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

    [DllImport("user32.dll")]
    static public extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

    public const int LWA_ALPHA = 0x2;
    public const int LWA_COLORKEY = 0x1;

    [DllImport("user32.dll")]
    static public extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    public struct BLENDFUNCTION
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    static public extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst,
        Point pptDst, Size psize, IntPtr hdcSrc, Point pptSrc, uint crKey,
        [In] BLENDFUNCTION pblend, uint dwFlags);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    static public extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst,
       IntPtr pptDst, IntPtr psize, IntPtr hdcSrc, IntPtr pptSrc, uint crKey,
       [In] IntPtr pblend, uint dwFlags);

    [DllImportAttribute("user32.dll")]
    public extern static IntPtr GetDC(IntPtr handle);
}
