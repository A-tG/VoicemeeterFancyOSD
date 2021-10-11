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

        public override void ReadIsNotifyChanges(bool isNotify)
        {
            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (m_api.GetParameter(m_command, out string val) == 0)
            {
                var oldVal = m_value;
                if (isNotify)
                {
                    Value = val;
                }
                else
                {
                    m_value = val;
                }
                OnValueRead(oldVal, val);
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
