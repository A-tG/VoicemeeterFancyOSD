using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AtgDev.Utils
{
    public static class OpenInOs
    {
        public static bool TryOpen(string path)
        {
            bool result = false;
            try
            {
                using Process p = new();
                ProcessStartInfo info = new()
                {
                    FileName = path,
                    UseShellExecute = true
                };
                p.StartInfo = info;
                _ = p.Start();

                result = true;
            }
            catch { }
            return result;
        }
    }
}
