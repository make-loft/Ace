using System;
using System.Globalization;
using System.Windows;
using Ace.Converters.Patterns;

namespace Ace.Converters
{
    public class NullConverter : AValueConverter
    {
        public static readonly DependencyProperty OnNullProperty = Register("OnNull");

        public object OnNull
        {
            get => GetValue(OnNullProperty);
            set => SetValue(OnNullProperty, value);
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            GetDefined(value == null ? OnNull : ByDefault, value);
    }
}