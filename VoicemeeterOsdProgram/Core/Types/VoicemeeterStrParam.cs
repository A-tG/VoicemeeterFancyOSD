using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterStrParam : VoicemeeterParameter<string>, IVmParamReadable
    {
        public VoicemeeterStrParam(RemoteApiExtender api, string command) : base(api, command) { }

        public void ReadIsIgnoreEvent(bool isIgnore)
        {
            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (m_api.GetParameter(m_command, out string val) == 0)
            {
                if (isIgnore)
                {
                    m_value = val;
                }
                else
                {
                    Value = val;
                }
                m_isInit = true;
            }
        }

        public void Write(string value)
        {
            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (m_api.SetParameter(m_command, value) == 0)
            {
                m_value = value;
            }
        }
    }
}
