using System;
using System.Globalization;
using System.Windows.Data;

namespace VoicemeeterOsdProgram.UiControls.Converters;

class RectVertHalfConverter : IMultiValueConverter
{
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
        return new System.Windows.Rect(0, 0, (double)value[0], (double)value[1] / 2);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
