using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace App3.Converters
{
    public class friendStatusToAction : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            switch (value)
            {
                case 0:
                    return "Add Friend";
                case 1:
                    return "Unadd";
                case 2:
                    return "Add back";
                case 3:
                    return "Unadd";
                default:
                    return "Unavailable";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
