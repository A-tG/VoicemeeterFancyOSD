using AtgDev.Voicemeeter;
using System;

namespace VoicemeeterOsdProgram.Core.Types;

public abstract class VoicemeeterParameterBase
{
    protected readonly RemoteApiExtender m_api;

    public string Name { get; private set; }

    public bool IsEnabled { get; set; } = true;

    internal int NameIndex { get; private set; } = 0;
    internal int NameLength { get; private set; } = 0;
    internal unsafe byte* NamesBuffer { get; private set; }
    internal unsafe byte* NameBuffer { get; private set; }
    internal bool IsOptimized { get; private set; } = false;

    public VoicemeeterParameterBase(RemoteApiExtender api, string command)
    {
        ArgumentNullException.ThrowIfNull(api);
        ArgumentException.ThrowIfNullOrWhiteSpace(command);

        m_api = api;
        Name = command;

        Read();
    }

    public void ReadNotifyChanges() => ReadIsNotifyChanges(true);

    public void Read() => ReadIsNotifyChanges(false);

    public abstract void ReadIsNotifyChanges(bool isNotify);

    public abstract void ClearEvents();


    internal unsafe void SetupNameBufferP(byte* buff, int index, int len)
    {
        NamesBuffer = buff;
        NameIndex = index;
        NameLength = len;
        NameBuffer = &buff[index];
        IsOptimized = true;
    }
}
