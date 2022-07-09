using System;
using System.Globalization;

namespace Ace.Markup.Patterns
{
	public delegate object Convert(ConvertArgs args);

	public struct ConvertArgs
	{
		public readonly object Value;
		public readonly object Parameter;
		public readonly Type TargetType;
		public readonly CultureInfo Culture;

		public ConvertArgs(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Value = value; TargetType = targetType; Parameter = parameter; Culture = culture;
		}
	}
}
