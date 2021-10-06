using System;
using System.Globalization;
using System.Windows.Data;

namespace VoicemeeterOsdProgram.UiControls.Converters
{
    class SliderSelectionHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var Value = (double)values[0];
            var Minimum = (double)values[1];
            var Maximum = (double)values[2];
            var ActualHeight = (double)values[3];
            return (1 - (Value - Minimum) / (Maximum - Minimum)) * ActualHeight;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
