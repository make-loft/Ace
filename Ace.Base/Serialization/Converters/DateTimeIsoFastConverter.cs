using System;
using System.Globalization;
using System.Text;

namespace Ace.Serialization.Converters
{
	public class DateTimeIsoFastConverter : Converter
	{	   
		public string DateTimeOffsetFormat = "O";
		public string DateTimeFormat = "O";
		public string TimeSpanFormat = "G";

		private readonly StringBuilder _builder = new StringBuilder(64);
		
		public override string Convert(object value)
		{
			_builder.Clear();
			switch (value)
			{
				case DateTime d:
					return ToIsoString(d);
				case DateTimeOffset d:
					return ToIsoString(d);
				case TimeSpan d:
					return d.ToString(TimeSpanFormat, ActiveCulture);
				default:
					return null;
			}
		}
		
		public override object Revert(string value, string typeCode)
		{
			switch (typeCode)
			{
				case "Uri":
					return new Uri(value);
				case "DateTime":
					return value.EndsWith("Z")
						? DateTime.Parse(value, ActiveCulture, DateTimeStyles.AdjustToUniversal)
						: DateTime.Parse(value, ActiveCulture);
				case "DateTimeOffset":
					return value.EndsWith("Z")
						? DateTimeOffset.Parse(value, ActiveCulture, DateTimeStyles.AdjustToUniversal)
						: DateTimeOffset.Parse(value, ActiveCulture);
				default:
					return NotParsed;
			}
		}
		
		// $"{d.Year:D4}-{d.Month:D2}-{d.Day:D2}T{d.Hour:D2}:{d.Minute:D2}:{d.Second:D2}.{d.Millisecond:D7}";
		public string ToIsoString(DateTime d) =>
			_builder
				.Append(d.Year.ToString("D4"),"-",d.Month.ToString("D2"),"-",d.Day.ToString("D2"))
				.Append("T")
				.Append(d.Hour.ToString("D2"),":",d.Minute.ToString("D2"),":",d.Second.ToString("D2"),".",d.Millisecond.ToString("D7"))
				.Append(d.Kind == DateTimeKind.Utc ? "Z" : ToShortString(TimeZoneInfo.Local.GetUtcOffset(d)))
				.ToString();

		// $"{d.Year:D4}-{d.Month:D2}-{d.Day:D2}T{d.Hour:D2}:{d.Minute:D2}:{d.Second:D2}.{d.Millisecond:D7}+{d.Offset}";
		public string ToIsoString(DateTimeOffset d) =>
			_builder
				.Append(d.Year.ToString("D4"),"-",d.Month.ToString("D2"),"-",d.Day.ToString("D2"))
				.Append("T")
				.Append(d.Hour.ToString("D2"),":",d.Minute.ToString("D2"),":",d.Second.ToString("D2"),".",d.Millisecond.ToString("D7"))
				.Append(ToShortString(d.Offset))
				.ToString();

		public string ToShortString(TimeSpan d) =>
			(d < TimeSpan.Zero ?  "-" : "+") +
			d.Hours.ToString("D2") + ":" + d.Minutes.ToString("D2");
	}
}
