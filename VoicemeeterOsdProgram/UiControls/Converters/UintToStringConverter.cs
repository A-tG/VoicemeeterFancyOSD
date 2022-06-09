using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VoicemeeterOsdProgram.UiControls.Converters
{
    internal class UintToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is uint v) && (targetType == typeof(string)))
            {
                return System.Convert.ToString(v, culture);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(uint)) throw new InvalidOperationException($"Unsupported type {targetType}");

            if (value is not string str) throw new InvalidOperationException("Value is not a string");

            str = FilterCharacters(str);
            if (uint.TryParse(str, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, culture, out uint number))
            {
                return number;
            }
            else
            {
                return 0;
            }
        }

        private string FilterCharacters(string str)
        {
            StringBuilder inSb = new(str);
            StringBuilder outSb = new();
            for (int i = 0; i < inSb.Length; i++)
            {
                var ch = inSb[i];
                if (char.IsWhiteSpace(ch)) continue;

                outSb.Append(ch);
            }
            return outSb.ToString();
        }
    }
}
