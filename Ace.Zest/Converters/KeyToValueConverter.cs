using System;
using System.Globalization;
using System.Windows;
using Ace.Converters.Patterns;

namespace Ace.Converters
{
	public enum KeySource
	{
		Manual, ConverterParameter
	}

	public class KeyToValueConverter : AValueConverter
	{
		public static readonly DependencyProperty KeyProperty = Register(nameof(Key));
		public static readonly DependencyProperty ValueProperty = Register(nameof(Value));

		public KeySource KeySource { get; set; } = KeySource.Manual;

		/* Manual Key */
		public object Key
		{
			get => GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			ChooseKey(parameter).Is(value, StringComparison) ? Value : ByDefault;

		private object ChooseKey(object parameter) => KeySource.Is(KeySource.Manual) ? Key : parameter;
	}
}