using System.IO;

namespace AtgDev.Utils.DirectoryInfoExtensions
{
    public static class DirectoryInfoExtension
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
