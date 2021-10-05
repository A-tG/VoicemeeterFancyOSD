using IniParser;
using IniParser.Model;
using System;
using System.Diagnostics;
using System.IO;

namespace VoicemeeterOsdProgram.Options
{
    public static class OptionsStorage
    {
        public static readonly OsdOptions Osd = new();

        private static readonly string m_path = @$"{AppDomain.CurrentDomain.BaseDirectory}config\config.ini";

        private static readonly FileIniDataParser m_parser = new();

        static OptionsStorage()
        {
            TryRead();
            TrySave();
        }

        public static void Init() { }

        public static bool TrySave()
        {
            bool result = false;
            try
            {
                IniData data = new();
                var directoryPath = Path.GetDirectoryName(m_path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                Osd.ToIniData(ref data);
                m_parser.WriteFile(m_path, data);

                result = true;
            }
            catch { }

            return result;
        }

        public static bool TryRead()
        {
            bool result = false;
            try
            {
                IniData data = m_parser.ReadFile(m_path);
                Osd.FromIniData(ref data);

                result = true;
            }
            catch { }

            return result;
        }
    }
}
