using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterNumParam : VoicemeeterParameterBase<float>
    {
        public VoicemeeterNumParam(RemoteApiExtender api, string command) : base(api, command) { }

        unsafe public override int GetParameter(out float val)
        {
            fixed (byte* command = m_nameBuffer)
            {
                return m_api.GetParameter((IntPtr)command, out val);
            }
        }

        unsafe public override int SetParameter(float value)
        {
            fixed (byte* command = m_nameBuffer)
            {
                return m_api.SetParameter((IntPtr)command, value);
            }
        }
    }
}
