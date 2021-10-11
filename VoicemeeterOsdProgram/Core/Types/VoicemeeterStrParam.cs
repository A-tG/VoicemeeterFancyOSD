using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterStrParam : VoicemeeterParameterBase<string>
    {
        public VoicemeeterStrParam(RemoteApiExtender api, string command) : base(api, command)
        {
            m_value = string.Empty;
            m_isInit = true;
        }

        public override int GetParameter(out string val)
        {
            return m_api.GetParameter(m_command, out val);
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
