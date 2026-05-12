using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Ace.Serialization.Converters;

public class SystemConverter : Converter
{
	public string DateTimeOffsetFormat = "O";
	public string DateTimeFormat = "O";
	public string TimeSpanFormat = "G";
	public string GuidFormat = "D";

	public override string Encode(object value) => value switch
	{
		Type t => t.GetFriendlyName(),
		Guid g => g.ToString(GuidFormat),
		TimeSpan ts => ts.ToString(TimeSpanFormat, ActiveCulture),
		DateTime dt => dt.ToString(DateTimeFormat, ActiveCulture),
		DateTimeOffset dto => dto.ToString(DateTimeOffsetFormat, ActiveCulture),
		object o => o.ToString(), // Uri, Enum, etc...
		_ => default
	};

	public override object Decode(string value, Type type) => type.Name switch
	{
		nameof(Uri) => new Uri(value),
		nameof(Guid) => Guid.Parse(value),
		nameof(TimeSpan) => TimeSpan.Parse(value, ActiveCulture),
		nameof(DateTime) => DateTime.Parse(value, ActiveCulture, GetDateTimeStyle(value)),
		nameof(DateTimeOffset) => DateTimeOffset.Parse(value, ActiveCulture, GetDateTimeStyle(value)),
		"RuntimeType" => Type.GetType(value),
		nameof(Object) => value,
		_ => Parse(value, type),
	};

	private DateTimeStyles GetDateTimeStyle(string value)
		=> value.EndsWith("Z") ? DateTimeStyles.AdjustToUniversal : DateTimeStyles.None;

	private static readonly Dictionary<Type, MethodInfo> TypeToParseWithFormatMethod = [];
	private static readonly Dictionary<Type, MethodInfo> TypeToParseMethod = [];

	private object Parse(string value, Type type)
	{
		if (type is null) return Undefined;
		if (type.IsEnum) return Enum.Parse(type, value, ignoreCase: true);

		var parseWithFormatMethod = TypeToParseWithFormatMethod.TryGetValue(type, out var methodWithFormat)
			? methodWithFormat
			: TypeToParseWithFormatMethod[type] = type.GetMethod("Parse", [TypeOf.String.Raw, typeof(IFormatProvider)])
			;

		if (parseWithFormatMethod.Is()) return parseWithFormatMethod.Invoke(null, [value, ActiveCulture]);

		var parseMethod = TypeToParseWithFormatMethod.TryGetValue(type, out var method)
			? method
			: TypeToParseMethod[type] = type.GetMethod("Parse", [TypeOf.String.Raw])
			;

		return parseMethod?.Invoke(null, [value]);
	}
}
