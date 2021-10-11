using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterNumParam : VoicemeeterParameterBase<float>
    {
        public VoicemeeterNumParam(RemoteApiExtender api, string command) : base(api, command) { }

        public override void ReadIsNotifyChanges(bool isNotify)
        {
            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (m_api.GetParameter(m_command, out float val) == 0)
            {
                if (isNotify)
                {
                    Value = val;
                }
                else
                {
                    m_value = val;
                }
                m_isInit = true;
            }
        }

        public void Write(float value)
        {
            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (m_api.SetParameter(m_command, value) == 0)
            {
                m_value = value;
            }
        }
    }
}
