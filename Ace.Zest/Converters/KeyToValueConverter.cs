using System;
using System.Globalization;
using System.Windows;
using Ace.Converters.Patterns;

namespace Ace.Converters
{
	public enum Source
	{
		Manual, ConverterParameter, PreferManual, PreferConverterParameter
	}

	public class KeyToValueConverter : AValueConverter
	{
		public static readonly DependencyProperty KeyProperty = Register(nameof(Key));
		public static readonly DependencyProperty ValueProperty = Register(nameof(Value));

		public Source KeySource { get; set; } = Source.Manual;
		public Source ValueSource { get; set; } = Source.Manual;

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
			Choose(KeySource, Key, parameter).Is(value, StringComparison) ? Choose(ValueSource, Value, parameter) : ByDefault;

		private static object Choose(Source source, object manual, object parameter) =>
			source.Is(Source.Manual) ? manual :
			source.Is(Source.ConverterParameter) ? parameter :
			source.Is(Source.PreferManual) ? (manual.Is(DependencyProperty.UnsetValue) ? parameter : manual) :
			parameter ?? manual;
	}
}