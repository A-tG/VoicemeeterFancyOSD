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

        public void ToIniData(ref IniData data)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                data[SectionName][prop.Name] = prop.GetValue(this).ToString();
            }
        }

        public void FromIniData(ref IniData data)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                ValidateOption(ref data, prop);
            }
        }

        private void ValidateOption(ref IniData data, PropertyInfo prop)
        {
            var name = prop.Name;
            var field = data[SectionName][name];
            try
            {
                var result = Convert.ChangeType(field, prop.PropertyType);
                if (result is not null)
                {
                    prop.SetValue(this, result);
                    return;
                }
            }
            catch { }
            data[SectionName][name] = prop.GetValue(this).ToString();
        }
    }
}
