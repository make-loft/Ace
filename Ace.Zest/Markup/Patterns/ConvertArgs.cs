using System.Globalization;

namespace Ace.Markup.Patterns;

public readonly struct ConvertArgs(object value, Type targetType, object parameter, CultureInfo culture)
{
	public object Value { get; } = value;
	public object Parameter { get; } = parameter;
	public Type TargetType { get; } = targetType;
	public CultureInfo Culture { get; } = culture;
}
