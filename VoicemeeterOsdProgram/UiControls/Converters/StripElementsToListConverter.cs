using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.UiControls.Converters
{
    internal class StripElementsToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new List<object>();
            if (value is HashSet<StripElements> set)
            {
                foreach (var item in set)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new HashSet<StripElements>();
            if (value is IEnumerable set)
            {
                foreach (var item in set)
                {
                    if (item is StripElements el) result.Add(el);
                }
            }
            return result;
        }
    }
}
