using System;
using System.Globalization;
using System.Windows;
using Ace.Converters.Patterns;

namespace Ace.Converters
{
	public class EqualsConverter : AValueConverter
	{
		public static readonly DependencyProperty OnEqualsProperty = Register("OnEquals");

		public object OnEquals
		{
			get => GetValue(OnEqualsProperty);
			set => SetValue(OnEqualsProperty, value);
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			EqualsAsStrings(value, parameter, StringComparison)
				? GetDefined(OnEquals, value)
				: GetDefined(ByDefault, value);

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			Equals(value, true) ? parameter : UndefinedValue;
	}
}