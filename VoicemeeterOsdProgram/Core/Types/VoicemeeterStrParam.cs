using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterStrParam : VoicemeeterParameterBase<string>
    {
        protected readonly char[] m_strGetValueBuffer = new char[512];

        public VoicemeeterStrParam(RemoteApiExtender api, string command) : base(api, command)
        {
            m_value = string.Empty;
            m_isInit = true;
        }

        unsafe public override int GetParameter(out string val)
        {
            val = "";
            fixed (char* buffPtr = m_strGetValueBuffer)
            {
                var ptr = (IntPtr)buffPtr;

                var res = m_api.GetParameter(m_nameBuffer, ptr);
                if (res == ResultCodes.Ok)
                {
                    val = new string(buffPtr);
                }
                return res;
            }
        }

        public override int SetParameter(string value) => m_api.SetParameter(m_nameBuffer, value);
    }
}
