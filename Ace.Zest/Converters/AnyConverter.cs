using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ace.Converters
{
	public class AnyConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty OnAnyProperty = DependencyProperty.Register(
			"OnAny", typeof(object), typeof(AnyConverter), new PropertyMetadata(default(object)));

		public object OnAny
		{
			get => GetValue(OnAnyProperty);
			set => SetValue(OnAnyProperty, value);
		}

		public static readonly DependencyProperty OnNotAnyProperty = DependencyProperty.Register(
			"OnNotAny", typeof(object), typeof(AnyConverter), new PropertyMetadata(default(object)));

		public object OnNotAny
		{
			get => GetValue(OnNotAnyProperty);
			set => SetValue(OnNotAnyProperty, value);
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value is IList list && list.Count > 0 ? OnAny : OnNotAny;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}