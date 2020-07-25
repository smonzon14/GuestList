using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace App3.Converters
{
    public class CellHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 580 : 180;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
