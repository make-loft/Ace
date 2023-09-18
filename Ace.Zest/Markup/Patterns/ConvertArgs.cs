using System;
using System.Globalization;

namespace Ace.Markup.Patterns
{
	public delegate object Convert(ConvertArgs args);

	public readonly struct ConvertArgs
	{
		public object Value { get; }
		public object Parameter { get; }
		public Type TargetType { get; }
		public CultureInfo Culture { get; }

		public ConvertArgs(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Value = value; TargetType = targetType; Parameter = parameter; Culture = culture;
		}
	}
}
