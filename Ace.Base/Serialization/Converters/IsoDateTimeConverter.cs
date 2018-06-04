using System;
using System.Text;

namespace Ace.Serialization.Converters
{
	public class IsoDateTimeConverter : Converter
	{
		public string DateTimeOffsetFormat = "O";
		public string DateTimeFormat = "O";

		private readonly StringBuilder _builder = new StringBuilder(64);

		public override string Convert(object value) => value.Match
		(
			(DateTime dt) => DateTimeFormat.Is("O") ? dt.ToIsoString(_builder.Clear()) : null,
			(DateTimeOffset dto) => DateTimeOffsetFormat.Is("O") ? dto.ToIsoString(_builder.Clear()) : null,
			(object o) => null,
			() => null
		);
	}

	public static class DateTimeExtensions
	{
		// $"{d.Year:D4}-{d.Month:D2}-{d.Day:D2}T{d.Hour:D2}:{d.Minute:D2}:{d.Second:D2}.{d.Millisecond:D7}";
		public static string ToIsoString(this DateTime d, StringBuilder builder) => builder
			.Append(d.Year.ToString("D4"), "-", d.Month.ToString("D2"), "-", d.Day.ToString("D2"))
			.Append("T")
			.Append(d.Hour.ToString("D2"), ":", d.Minute.ToString("D2"), ":", d.Second.ToString("D2"))
			.Append(".", d.Millisecond.ToString("D7"))
			.Append(d.Kind == DateTimeKind.Utc ? "Z" : TimeZoneInfo.Local.GetUtcOffset(d).ToShortString(builder))
			.ToString();

		// $"{d.Year:D4}-{d.Month:D2}-{d.Day:D2}T{d.Hour:D2}:{d.Minute:D2}:{d.Second:D2}.{d.Millisecond:D7}+{d.Offset}";
		public static string ToIsoString(this DateTimeOffset d, StringBuilder builder) => builder
			.Append(d.Year.ToString("D4"), "-", d.Month.ToString("D2"), "-", d.Day.ToString("D2"))
			.Append("T")
			.Append(d.Hour.ToString("D2"), ":", d.Minute.ToString("D2"), ":", d.Second.ToString("D2"))
			.Append(".", d.Millisecond.ToString("D7"))
			.Append(d.Offset.ToShortString(builder))
			.ToString();

		public static string ToShortString(this TimeSpan d, StringBuilder builder) => builder
			.Append(d < TimeSpan.Zero ? "-" : "+", d.Hours.ToString("D2"), ":", d.Minutes.ToString("D2"))
			.ToString();
	}
}