using System;
using System.IO;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Helpers;

static public class IOAccessCheck
{
    public static Exception LastException { get; private set; }

    static public async Task<bool> TryCreateRandomFileAsync(string folderPath)
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
            LastException = ex;
        }
        return false;
    }

    static public bool TryCreateRandomDirectory(string inFolder)
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
            LastException = ex;
        }
        return false;
    }
}
