using System.Runtime.InteropServices;

namespace VoicemeeterOsdProgram.Core.Types;

unsafe static internal class VmParamsMemory
{
    internal static byte* ParamNamesBuffer { get; private set; }

    internal static void Allocate(nuint len)
    {
        ParamNamesBuffer = (byte*)NativeMemory.Realloc(ParamNamesBuffer, len);
    }
}
