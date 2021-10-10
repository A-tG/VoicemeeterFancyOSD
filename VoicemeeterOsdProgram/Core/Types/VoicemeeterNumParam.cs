using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterNumParam : VoicemeeterParameter<float>, IVmParamReadable
    {

        public VoicemeeterNumParam(RemoteApiExtender api, string command) : base(api, command) { }

        public void ReadIsIgnoreEvent(bool isIgnore)
        {
            if ((m_api is null) || string.IsNullOrEmpty(m_command)) return;

            if (m_api.GetParameter(m_command, out float val) == 0)
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
