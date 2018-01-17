using System;
using System.Globalization;
using System.Windows.Data;

namespace Ace.Converters.Patterns
{
	public class ConverterEventArgs : EventArgs
	{
		public object ConvertedValue { get; set; }
		public object Value { get; private set; }
		public Type TargetType { get; private set; }
		public object Parameter { get; private set; }
		public CultureInfo Culture { get; private set; }

		public ConverterEventArgs(object value, Type targetType, object parameter, CultureInfo culture)
		{
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

	//public interface IInlineConverter : IValueConverter
	//{
	//	event Func<object, Type, object, CultureInfo, object> Converting;
	//	event Func<object, Type, object, CultureInfo, object> ConvertingBack;
	//}
}