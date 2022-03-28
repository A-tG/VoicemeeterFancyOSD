using AtgDev.Voicemeeter;
using System;
using System.Runtime.InteropServices;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterStrParam : VoicemeeterParameterBase<string>
    {
        protected readonly IntPtr m_strGetValueBuffer;

        public VoicemeeterStrParam(RemoteApiExtender api, string command) : base(api, command)
        {
            m_value = string.Empty;
            m_isInit = true;
            m_strGetValueBuffer = Marshal.AllocHGlobal(1024);
        }

        public override int GetParameter(out string val)
        {
            val = "";
            var res = m_api.GetParameter(m_nameBuffer, m_strGetValueBuffer);
            if (res == ResultCodes.Ok)
            {
                val = Marshal.PtrToStringUni(m_strGetValueBuffer) ?? "";
            }
            return res;
        }

        public override int SetParameter(string value) => m_api.SetParameter(m_nameBuffer, value);

        ~VoicemeeterStrParam()
        {
            Marshal.FreeHGlobal(m_strGetValueBuffer);
        }
    }
}
