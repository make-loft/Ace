using System;
using System.Globalization;

namespace Ace.Serialization.Converters
{
	public class ComplexConverter : Converter
	{
		public string DateTimeOffsetFormat = "O";
		public string DateTimeFormat = "O";
		public string TimeSpanFormat = "G";
		public string GuidFormat = "D";

		public override string Convert(object value) => value.Match
		(
			(Type t) => t.AssemblyQualifiedName,
			(Guid g) => g.ToString(GuidFormat),
			(TimeSpan ts) => ts.ToString(TimeSpanFormat, ActiveCulture),
			(DateTime dt) => dt.ToString(DateTimeFormat, ActiveCulture),
			(DateTimeOffset dto) => dto.ToString(DateTimeOffsetFormat, ActiveCulture),
			(object o) => o.ToString(), // Uri, Enum, etc...
			() => null
		);

		public override object Revert(string value, string typeKey) =>
			typeKey.Is("Uri") ? new Uri(value) :
			typeKey.Is("Guid") ? Guid.Parse(value) :
			typeKey.Is("TimeSpan") ? TimeSpan.Parse(value, ActiveCulture) :
			typeKey.Is("DateTime") ? DateTime.Parse(value, ActiveCulture, GetDateTimeStyle(value)) :
			typeKey.Is("DateTimeOffset") ? DateTimeOffset.Parse(value, ActiveCulture, GetDateTimeStyle(value)) :
			typeKey.Is("RuntimeType") ? Type.GetType(value) :
			TryParse(value, typeKey);

		private DateTimeStyles GetDateTimeStyle(string value) =>
			value.EndsWith("Z") ? DateTimeStyles.AdjustToUniversal : DateTimeStyles.None;

		private object TryParse(string value, string typeKey)
		{
			var type = Type.GetType(typeKey) ?? Type.GetType($"System.{typeKey}");
			if (type is null) return null;
			if (type.IsEnum) return Enum.Parse(type, value, true);

			var parseWithFormatMethod = type.GetMethod("Parse", new[] {TypeOf.String.Raw, typeof(IFormatProvider)});
			if (parseWithFormatMethod.Is()) return parseWithFormatMethod.Invoke(null, new object[] {value, ActiveCulture});

			var parseMethod = type.GetMethod("Parse", new[] {TypeOf.String.Raw});
			return parseMethod?.Invoke(null, new object[] {value});
		}
	}
}