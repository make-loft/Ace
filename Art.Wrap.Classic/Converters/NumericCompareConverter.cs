using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Art.Converters
{
    public class NumericCompareConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty OnEqualsProperty = DependencyProperty.Register(
            "OnEquals", typeof(object), typeof(NumericCompareConverter), new PropertyMetadata(default(object)));

        public object OnEquals
        {
            get => GetValue(OnEqualsProperty);
            set => SetValue(OnEqualsProperty, value);
        }

        public static readonly DependencyProperty OnGreateProperty = DependencyProperty.Register(
            "OnGreate", typeof(object), typeof(NumericCompareConverter), new PropertyMetadata(default(object)));

        public object OnGreate
        {
            get => GetValue(OnGreateProperty);
            set => SetValue(OnGreateProperty, value);
        }

        public static readonly DependencyProperty OnLessProperty = DependencyProperty.Register(
            "OnLess", typeof(object), typeof(NumericCompareConverter), new PropertyMetadata(default(object)));

        public object OnLess
        {
            get => GetValue(OnLessProperty);
            set => SetValue(OnLessProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = decimal.Parse((value ?? 0).ToString(), culture);
            var p = decimal.Parse((parameter ?? 0).ToString(), culture);
            return v < p ? OnLess : v > p ? OnGreate : OnEquals;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}