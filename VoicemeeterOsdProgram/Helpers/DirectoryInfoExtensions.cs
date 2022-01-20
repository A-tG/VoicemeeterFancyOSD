using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtgDev.Utils.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static ulong GetSize(this DirectoryInfo dir)
        {
            ulong size = 0;
            size += dir.GetFilesOnlySize();
            foreach (var d in dir.GetDirectories())
            {
                size += d.GetSize();
            }
            return size;
        }

        public static ulong GetFilesOnlySize(this DirectoryInfo dir)
        {
            ulong size = 0;
            foreach (var file in dir.GetFiles())
            {
                size += (ulong)file.Length;
            }
            return size;
        }
    }
}
