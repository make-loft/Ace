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

		public override string Convert(object value)
		{
			switch (value)
			{
				case Enum e:
					return e.ToString();
				case Type t:
					return t.AssemblyQualifiedName;
				case Uri u:
					return u.ToString();
				case Guid d:
					return d.ToString(GuidFormat);
				case DateTime d:
					return d.ToString(DateTimeFormat, ActiveCulture);
				case DateTimeOffset d:
					return d.ToString(DateTimeOffsetFormat, ActiveCulture);
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
					var type = Type.GetType(typeCode) ?? Type.GetType("System." + typeCode);
					if (type == null) return null;
					if (type.IsEnum) return Enum.Parse(type, value, true);

					var parseMethodFormatted = type.GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider) });
					if (parseMethodFormatted != null) return parseMethodFormatted.Invoke(null, new object[] { value, ActiveCulture });

					var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
					return parseMethod?.Invoke(null, new object[] { value });
			}
		}
	}
}