using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Art.Converters
{
    public class EqualsConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty OnEqualProperty = DependencyProperty.Register(
            "OnEqual", typeof (object), typeof (EqualsConverter), new PropertyMetadata(default(object)));

        public object OnEqual
        {
            get { return GetValue(OnEqualProperty); }
            set { SetValue(OnEqualProperty, value); }
        }

        public static readonly DependencyProperty OnNotEqualProperty = DependencyProperty.Register(
            "OnNotEqual", typeof (object), typeof (EqualsConverter), new PropertyMetadata(default(object)));

        public object OnNotEqual
        {
            get { return GetValue(OnNotEqualProperty); }
            set { SetValue(OnNotEqualProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter is string
                ? parameter.ToString().Split().Contains(value)
                    ? OnEqual
                    : OnNotEqual
                : Equals(value, parameter)
                    ? OnEqual
                    : OnNotEqual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Equals(value, true) ? parameter : null;
        }
    }
}