using System;
using System.Globalization;
using System.Windows;

namespace Ace.Converters
{
	public class NumericCompareConverter : EqualsConverter
	{
		public static readonly DependencyProperty OnGreateProperty = Register("OnGreate");
		public static readonly DependencyProperty OnLessProperty = Register("OnLess");

		public object OnGreate
		{
			get => GetValue(OnGreateProperty);
			set => SetValue(OnGreateProperty, value);
		}

		public object OnLess
		{
			get => GetValue(OnLessProperty);
			set => SetValue(OnLessProperty, value);
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value == null || parameter == null ||
			!decimal.TryParse(value.ToString(), out var v) ||
			!decimal.TryParse(value.ToString(), out var p) ? GetDefined(ByDefault, value) :
			v > p ? GetDefined(OnGreate, value) :
			v < p ? GetDefined(OnLess, value) :
			GetDefined(OnEquals, value);
	}
}