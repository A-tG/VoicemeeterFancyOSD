using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    public class VoicemeeterParameter
    {
        private readonly string m_command;
        private readonly RemoteApiExtender m_api;
        private bool m_isInit;
        private float m_value;

        public VoicemeeterParameter(RemoteApiExtender api, string command)
        {
            m_api = api;
            m_command = command;
        }

        public float Value
        {
            get => m_value;
            private set
            {
                if (m_isInit)
                {
                    if (value != m_value) OnReadValueChanged(m_value, value);
                }
                else
                {
                    m_isInit = true;
                }
                m_value = value;
            }
        }

        public void Read()
        {
            if (m_api is null) return;

            var res = m_api.GetParameter(m_command, out float val);
            if (res == 0)
            {
                Value = val;
            }
        }

        public void ReadNoEvent()
        {
            if (m_api is null) return;

            var res = m_api.GetParameter(m_command, out float val);
            if (res == 0)
            {
                m_value = val;
                if (!m_isInit) m_isInit = true;
            }
        }

        public event EventHandler<ValuesPair<float>> ReadValueChanged;

        private void OnReadValueChanged(float oldVal, float newVal)
        {
            ValuesPair<float> values = new(oldVal, newVal);
            ReadValueChanged?.Invoke(this, values);
        }
    }
}
