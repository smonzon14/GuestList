using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace App3.Converters
{
    public class NullToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
