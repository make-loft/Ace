using System;
using System.Globalization;
using System.Linq;

namespace Ace.Serialization.Converters
{
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
			"Uri" => new Uri(value),
			"Guid" => Guid.Parse(value),
			"TimeSpan" => TimeSpan.Parse(value, ActiveCulture),
			"DateTime" => DateTime.Parse(value, ActiveCulture, GetDateTimeStyle(value)),
			"DateTimeOffset" => DateTimeOffset.Parse(value, ActiveCulture, GetDateTimeStyle(value)),
			"RuntimeType" => Type.GetType(value),
			_ => TryParse(value, type),
		};

		private DateTimeStyles GetDateTimeStyle(string value) =>
			value.EndsWith("Z") ? DateTimeStyles.AdjustToUniversal : DateTimeStyles.None;

		private object TryParse(string value, Type type)
		{
			if (type is null) return Undefined;
			if (type.IsEnum) return Enum.Parse(type, value, true);

			var parseWithFormatMethod = type.GetMethod("Parse", new[] { TypeOf.String.Raw, typeof(IFormatProvider) });
			if (parseWithFormatMethod.Is()) return parseWithFormatMethod.Invoke(null, new object[] { value, ActiveCulture });

			var parseMethod = type.GetMethod("Parse", new[] { TypeOf.String.Raw });
			return parseMethod?.Invoke(null, new object[] { value });
		}
	}
}