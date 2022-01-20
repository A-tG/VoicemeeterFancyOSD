using System.Diagnostics;
using TopmostApp.Interop;

namespace AtgDev.Utils.Extensions
{
    public static class ProcessExtensions
    {
        public static void RequestKill(this Process p)
        {
            foreach (ProcessThread t in p.Threads)
            {
                // when Process.CloseMainWindow() doesnt work because there is no window, and process should not be terminated abnormaly
                NativeMethods.PostThreadMessage(t.Id, (uint)WindowMessage.WM_CLOSE, 0, 0);
            }
        }
    }
}
