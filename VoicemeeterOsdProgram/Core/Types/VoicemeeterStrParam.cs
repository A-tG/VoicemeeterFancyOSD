using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types;

public class VoicemeeterStrParam : VoicemeeterParameterBase<string>
{
    protected readonly char[] m_strGetValueBuffer = new char[512];

    public VoicemeeterStrParam(RemoteApiExtender api, string command) : base(api, command)
    {
        m_value = string.Empty;
    }

    unsafe public override int GetParameter(out string val)
    {
        val = "";
        fixed (char* buffPtr = m_strGetValueBuffer)
        {
            fixed (byte* command = m_nameBuffer)
            {
                var res = m_api.GetParameter((IntPtr)command, (IntPtr)buffPtr);
                if (res == ResultCodes.Ok)
                {
                    val = new string(buffPtr);
                }
                return res;
            }
        }
    }

    unsafe public override int SetParameter(string value)
    {
        fixed (byte* command = m_nameBuffer)
        {
            return m_api.SetParameter((IntPtr)command, value);
        }
    }
}
