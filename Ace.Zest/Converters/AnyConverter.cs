using System;
using System.Collections;
using System.Globalization;
using System.Windows;

namespace Ace.Converters
{
	public class AnyConverter : NullConverter
	{
		public static readonly DependencyProperty OnAnyProperty = Register("OnAny");

		public object OnAny
		{
			get => GetValue(OnAnyProperty);
			set => SetValue(OnAnyProperty, value);
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value is IList list && list.Count > 0
				? GetDefined(OnAny, value)
				: GetDefined(value == null ? OnNull : ByDefault, value);
	}
}