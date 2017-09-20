using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Aero.Converters
{
    public class NumericCompareConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty OnEqualsProperty = DependencyProperty.Register(
            "OnEquals", typeof (object), typeof (NumericCompareConverter), new PropertyMetadata(default(object)));

        public object OnEquals
        {
            get { return GetValue(OnEqualsProperty); }
            set { SetValue(OnEqualsProperty, value); }
        }

        public static readonly DependencyProperty OnGreateProperty = DependencyProperty.Register(
            "OnGreate", typeof (object), typeof (NumericCompareConverter), new PropertyMetadata(default(object)));

        public object OnGreate
        {
            get { return GetValue(OnGreateProperty); }
            set { SetValue(OnGreateProperty, value); }
        }

        public static readonly DependencyProperty OnLessProperty = DependencyProperty.Register(
            "OnLess", typeof (object), typeof (NumericCompareConverter), new PropertyMetadata(default(object)));

        public object OnLess
        {
            get { return GetValue(OnLessProperty); }
            set { SetValue(OnLessProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var v = decimal.Parse(value.ToString(), new CultureInfo(culture));
            var p = decimal.Parse(parameter.ToString(), new CultureInfo(culture));
            return v < p ? OnLess : v > p ? OnGreate : OnEquals;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}