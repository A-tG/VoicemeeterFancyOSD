using AtgDev.Voicemeeter;
using System.Text;

namespace VoicemeeterOsdProgram.Core.Types;

public abstract class VoicemeeterParameterBase
{
    protected readonly string m_name;
    protected readonly RemoteApiExtender m_api;

    protected readonly byte[] m_nameBuffer;

    public VoicemeeterParameterBase(RemoteApiExtender api, string command)
    {
        m_api = api;
        m_name = command;

        m_nameBuffer = GetNullTermAsciiBuffFromString(command);

        Read();
    }

    public string Name { get => m_name; }

    public bool IsEnabled { get; set; } = true;

    public void ReadNotifyChanges()
    {
        ReadIsNotifyChanges(true);
    }

    public void Read()
    {
        ReadIsNotifyChanges(false);
    }

    public abstract void ReadIsNotifyChanges(bool isNotify);

    public abstract void ClearEvents();

    protected byte[] GetNullTermAsciiBuffFromString(string str)
    {
        var len = str.Length;
        var buff = new byte[str.Length + 1];
        Encoding.ASCII.GetBytes(str, 0, len, buff, 0);
        buff[^1] = 0;
        return buff;
    }

}
