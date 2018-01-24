using System;
using System.Globalization;
using System.Windows;

namespace Ace.Converters
{
	public class BooleanConverter : NullConverter
	{
		public static readonly DependencyProperty OnTrueProperty = Register("OnTrue");
		public static readonly DependencyProperty OnFalseProperty = Register("OnFalse");

		public object OnTrue
		{
			get => GetValue(OnTrueProperty);
			set => SetValue(OnTrueProperty, value);
		}

		public object OnFalse
		{
			get => GetValue(OnFalseProperty);
			set => SetValue(OnFalseProperty, value);
		}

		private static readonly string TrueString = true.ToString();
		private static readonly string FalseString = false.ToString();

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			Equals(value, false) ? GetDefined(OnFalse, false) :
			Equals(value, true) ? GetDefined(OnTrue, true) :
			Equals(value, null) ? GetDefined(OnNull, null) :
			EqualsAsStrings(value, FalseString, StringComparison) ? GetDefined(OnFalse, false) :
			EqualsAsStrings(value, TrueString, StringComparison) ? GetDefined(OnTrue, true) :
			GetDefined(ByDefault, value);
	}
}