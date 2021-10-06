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
        public bool IsInteractable { get; set; } = false;
        public int DurationMs { get; set; } = 2000;

        public void ToIniData(ref IniData data)
        {
            data["Osd"][nameof(IsShowOnlyIfVoicemeeterHidden)] = IsShowOnlyIfVoicemeeterHidden.ToString();
            data["Osd"][nameof(IsInteractable)] = IsInteractable.ToString();
            data["Osd"][nameof(DurationMs)] = DurationMs.ToString();
        }

        public void FromIniData(ref IniData data)
        {
            bool result;
            int resultInt;

            var field = data["Osd"][nameof(IsShowOnlyIfVoicemeeterHidden)];
            if (bool.TryParse(field, out result))
            {
                IsShowOnlyIfVoicemeeterHidden = result;
            } else
            {
                data["Osd"][nameof(IsShowOnlyIfVoicemeeterHidden)] = IsShowOnlyIfVoicemeeterHidden.ToString();
            }

            field = data["Osd"][nameof(IsInteractable)];
            if (bool.TryParse(field, out result))
            {
                IsInteractable = result;
            }
            else
            {
                data["Osd"][nameof(IsInteractable)] = IsInteractable.ToString();
            }

            field = data["Osd"][nameof(DurationMs)];
            if (int.TryParse(field, out resultInt))
            {
                DurationMs = resultInt;
            }
            else
            {
                data["Osd"][nameof(DurationMs)] = DurationMs.ToString();
            }
        }
    }
}
