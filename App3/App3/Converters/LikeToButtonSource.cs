using System;
using System.Globalization;
using Xamarin.Forms;

namespace App3.Converters
{
    class LikeToButtonSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool liked)
            {
                return liked ? "button_liked" : "button_like";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
