using AtgDev.Voicemeeter;
using System;
using System.Linq;

namespace VoicemeeterOsdProgram.Core.Types;

public abstract class VoicemeeterParameterBase<T> : VoicemeeterParameterBase
{
    protected T m_value; // can be null, initialize in derived class

    public VoicemeeterParameterBase(RemoteApiExtender api, string command) : base(api, command) { }

    public T Value
    {
        get => m_value;
        protected set
        {
            if (m_value.Equals(value)) return;

            m_value = value;
            OnReadValueChanged(m_value, value);
        }
    }

    public abstract int GetParameter(out T value);

    public abstract int SetParameter(T value);

    public override void ReadIsNotifyChanges(bool isNotify)
    {
        if (!IsEnabled) return;

        if (GetParameter(out T val) != ResultCodes.Ok) return;

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

    public void Write(T value)
    {
        if (!IsEnabled) return;

        if (SetParameter(value) == ResultCodes.Ok)
        {
            m_value = value;
        }
    }

    public override void ClearEvents()
    {
        if (ReadValueChanged is not null)
        {
            foreach (var del in ReadValueChanged.GetInvocationList().Cast<EventHandler<ValOldNew<T>>>())
            {
                ReadValueChanged -= del;
            }
        }

        if (ValueRead is not null)
        {
            foreach (var del in ValueRead.GetInvocationList().Cast<EventHandler<ValOldNew<T>>>())
            {
                ValueRead -= del;
            }
        }
    }

    public event EventHandler<ValOldNew<T>> ReadValueChanged;
    public event EventHandler<ValOldNew<T>> ValueRead;

    private void OnReadValueChanged(T oldVal, T newVal)
    {
        if (!IsEnabled) return;

        ValOldNew<T> values = new(oldVal, newVal);
        ReadValueChanged?.Invoke(this, values);
    }

    protected void OnValueRead(T oldVal, T newVal)
    {
        if (!IsEnabled) return;

        ValOldNew<T> values = new(oldVal, newVal);
        ValueRead?.Invoke(this, values);
    }
}
