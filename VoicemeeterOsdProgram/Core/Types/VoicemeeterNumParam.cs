using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterNumParam : VoicemeeterParameterBase<float>
    {
<<<<<<< Updated upstream
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
=======
        return IsOptimized ? 
            m_api.GetParameter((IntPtr)NameBuffer, out val) : 
            m_api.GetParameter(Name, out val);
    }

    unsafe public override int SetParameter(float value)
    {
        return IsOptimized ? 
            m_api.SetParameter((IntPtr)NameBuffer, value) : 
            m_api.SetParameter(Name, value);
>>>>>>> Stashed changes
    }
}
