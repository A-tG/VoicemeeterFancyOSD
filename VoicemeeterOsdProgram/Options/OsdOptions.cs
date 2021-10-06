using IniParser.Model;
using System;
using System.Reflection;

namespace VoicemeeterOsdProgram.Options
{
    public class OsdOptions
    {
        private const string SectionName = "Osd";

        public bool IsShowOnlyIfVoicemeeterHidden { get; set; } = true;
        public bool IsInteractable { get; set; } = false;
        public int DurationMs { get; set; } = 2000;
    }
}
