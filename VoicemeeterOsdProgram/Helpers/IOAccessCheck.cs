using System;
using System.IO;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Helpers
{
    static public class IOAccessCheck
    {
        static public async Task<bool> TryCreateRandomFileAsync(string folderPath, ref Exception e)
        {
            try
            {
                var p = Path.Combine(folderPath, Guid.NewGuid().ToString());
                using var fs = File.Create(p);
                await fs.DisposeAsync();
                File.Delete(p);
                return true;
            }
            catch (Exception ex)
            {
                e = ex;
            }
            return false;
        }

        static public bool TryCreateRandomDirectory(string inFolder, ref Exception e)
        {
            try
            {
                var p = Path.Combine(inFolder, Guid.NewGuid().ToString());
                var dir = Directory.CreateDirectory(p);
                if (dir.Exists)
                {
                    dir.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                e = ex;
            }
            return false;
        }
    }
}
