using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types
{
    class VoicemeeterParameter
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
                if (value != m_value)
                {
                    if (m_isInit)
                    {
                        OnValueChanged(m_value, value);
                    }
                    m_isInit = true;
                    m_value = value;
                }
            }
        }

        public void Read()
        {
            var res = m_api.GetParameter(m_command, out float val);
            if (res == 0)
            {
                Value = val;
            }
        }

        public event EventHandler<ValuesPair<float>> ValueChanged;

        private void OnValueChanged(float oldVal, float newVal)
        {
            ValuesPair<float> values = new(oldVal, newVal);
            ValueChanged?.Invoke(this, values);
        }
    }
}
