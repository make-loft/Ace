using System;
using System.Globalization;
using System.Windows.Data;

namespace Ace.Converters.Patterns
{
	public class ConverterEventArgs : EventArgs
	{
		public object ConvertedValue { get; set; }
		public object Value { get; }
		public Type TargetType { get; }
		public object Parameter { get; }
		public CultureInfo Culture { get; }

		public ConverterEventArgs(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ConvertedValue = value;
			TargetType = targetType;
			Parameter = parameter;
			Culture = culture;
			Value = value;
		}
	}

	public interface IInlineConverter : IValueConverter
	{
		event EventHandler<ConverterEventArgs> Converting;
		event EventHandler<ConverterEventArgs> ConvertingBack;
	}
}