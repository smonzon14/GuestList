using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace App3.Converters
{
    
    class ObjectToFalse<T> : IValueConverter
    {
        public T Object { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((T)value).Equals(Object);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
