using AtgDev.Voicemeeter;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterNumParam : VoicemeeterParameterBase<float>
    {
        public VoicemeeterNumParam(RemoteApiExtender api, string command) : base(api, command) { }

        public override int GetParameter(out float val) => m_api.GetParameter(m_nameBuffer, out val);

        public override int SetParameter(float value) => m_api.SetParameter(m_nameBuffer, value);
    }
}
