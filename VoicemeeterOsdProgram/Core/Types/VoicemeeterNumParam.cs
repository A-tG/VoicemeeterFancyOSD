using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types;

public class VoicemeeterNumParam(RemoteApiExtender api, string command) : VoicemeeterParameterBase<float>(api, command)
{
    unsafe public override int GetParameter(out float val) => IsOptimized ?
            m_api.GetParameter((IntPtr)NameBuffer, out val) :
            m_api.GetParameter(Name, out val);

    unsafe public override int SetParameter(float value) => IsOptimized ?
                m_api.SetParameter((IntPtr)NameBuffer, value) :
                m_api.SetParameter(Name, value);
}
