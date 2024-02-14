using AtgDev.Voicemeeter;
using System;
using System.Text;

namespace VoicemeeterOsdProgram.Core.Types
{
<<<<<<< Updated upstream
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
=======
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
        if (string.IsNullOrWhiteSpace(command)) throw new ArgumentNullException(nameof(command));

        m_api = api;
        Name = command;
>>>>>>> Stashed changes

    }
<<<<<<< Updated upstream
=======

    public void ReadNotifyChanges() => ReadIsNotifyChanges(true);

    public void Read() => ReadIsNotifyChanges(false);

    public abstract void ReadIsNotifyChanges(bool isNotify);

    public abstract void ClearEvents();

    protected static byte[] GetNullTermAsciiBuffFromString(string str)
    {
        var len = str.Length;
        var buff = new byte[str.Length + 1];
        Encoding.ASCII.GetBytes(str, 0, len, buff, 0);
        buff[^1] = 0;
        return buff;
    }

    internal unsafe void SetupNameBufferP(byte* buff, int index, int len)
    {
        NamesBuffer = buff;
        NameIndex = index;
        NameLength = len;
        NameBuffer = &buff[index];
        IsOptimized = true;
    }
>>>>>>> Stashed changes
}
