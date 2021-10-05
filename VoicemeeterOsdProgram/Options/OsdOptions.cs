using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoicemeeterOsdProgram.Options
{
    public class OsdOptions
    {
        public bool IsShowOnlyIfVoicemeeterHidden { get; set; } = true;

        public void ToIniData(ref IniData data)
        {
            data["Osd"][nameof(IsShowOnlyIfVoicemeeterHidden)] = IsShowOnlyIfVoicemeeterHidden.ToString();
        }

        public void FromIniData(ref IniData data)
        {
            var field = data["Osd"][nameof(IsShowOnlyIfVoicemeeterHidden)];
            if (bool.TryParse(field, out bool result))
            {
                IsShowOnlyIfVoicemeeterHidden = result;
            } else
            {
                data["Osd"][nameof(IsShowOnlyIfVoicemeeterHidden)] = IsShowOnlyIfVoicemeeterHidden.ToString();
            }
        }
    }
}
