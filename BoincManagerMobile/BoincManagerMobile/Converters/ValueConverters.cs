using System;
using System.Globalization;
using Xamarin.Forms;

namespace BoincManagerMobile.Converters
{
    public sealed class FractionToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("P2", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
