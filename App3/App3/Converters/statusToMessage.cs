using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace App3.Converters
{
    public class statusToMessage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string statusString;
            switch (value)
            {
                case 1:
                    statusString = "Going Out";
                    break;
                case 2:
                    statusString = "Staying In";
                    break;
                default:
                    statusString = "Offline";
                    break;
            }
            return statusString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
