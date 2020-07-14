using System;
using System.Globalization;
using Xamarin.Forms;

namespace App3.Converters
{
    public class ActionToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ("Add Friend".Equals(value)) return Color.DeepSkyBlue;
            if ("Add Back".Equals(value)) return Color.Goldenrod;
            else return Color.Silver;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
