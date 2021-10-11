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

        public override int GetParameter(out string val) => m_api.GetParameter(m_command, out val);

        public override int SetParameter(string value) => m_api.SetParameter(m_command, value);
    }
}
