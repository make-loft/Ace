using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ace.Serialization.Converters
{
	public class NumberConverter : Converter
	{
		public string IntegerNumbersFormat = null; //"G"; /* 'null' for best performance */
		public string RealNumbersFormat = "G";
		public bool AppendSyffixes = true;

		public Dictionary<Type, string> TypeToSyffix = new Dictionary<Type, string>
		{
			{typeof(byte), "B"},
			{typeof(char), "C"},
			{typeof(int), null}, /* "I" */
			{typeof(uint), "U"},
			{typeof(long), "L"},
			{typeof(ulong), "UL"},
			{typeof(float), "F"},
			{typeof(double), null}, /* "D" */
			{typeof(decimal), "M"},
			{typeof(IntPtr), "P"},
			{typeof(UIntPtr), "UP"}
		};

		public override string Convert(object value)
		{
			var convertedValue = ToStringConvert(value);
			if (convertedValue == null) return null;
			var decimalSeparator = ActiveCulture.NumberFormat.NumberDecimalSeparator;
			if ((value is double || value is float || value is decimal)
				&& convertedValue.IndexOf(decimalSeparator, StringComparison.OrdinalIgnoreCase) < 0)
				convertedValue += decimalSeparator + "0";
			var suffix = AppendSyffixes && TypeToSyffix.TryGetValue(value.GetType(), out var s) ? s : null;
			return suffix == null ? convertedValue : convertedValue + suffix;
		}

		protected string ToStringConvert(object value)
		{
			switch (value)
			{
				case sbyte b:
					return b.ToString(IntegerNumbersFormat, ActiveCulture);
				case byte b:
					return b.ToString(IntegerNumbersFormat, ActiveCulture);
				case char c:
					return ((int) c).ToString(IntegerNumbersFormat, ActiveCulture);
				case short i:
					return i.ToString(IntegerNumbersFormat, ActiveCulture);
				case ushort i:
					return i.ToString(IntegerNumbersFormat, ActiveCulture);
				case int i:
					return i.ToString(IntegerNumbersFormat, ActiveCulture);
				case uint i:
					return i.ToString(IntegerNumbersFormat, ActiveCulture);
				case long i:
					return i.ToString(IntegerNumbersFormat, ActiveCulture);
				case ulong i:
					return i.ToString(IntegerNumbersFormat, ActiveCulture);
				case float n:
					return n.ToString(RealNumbersFormat, ActiveCulture);
				case double n:
					return n.ToString(RealNumbersFormat, ActiveCulture);
				case decimal m:
					return m.ToString(RealNumbersFormat, ActiveCulture);
				case IntPtr i:
					return i.ToString();
				case UIntPtr i:
					return i.ToString();
				default:
					return null;
			}
		}

		public override object Revert(string value, string typeCode)
		{
			if (value.Length > 0 && char.IsDigit(value[value.Length - 1]))
			{
				if (int.TryParse(value, NumberStyles.Any, ActiveCulture, out var i)) return i;
				if (double.TryParse(value, NumberStyles.Any, ActiveCulture, out var r)) return r;
			}

			var number = value.ToUpper();
			if (value.EndsWith("B") && byte.TryParse(number.Substring(0, number.Length - 1), out var b)) return b;
			if (value.EndsWith("C") && int.TryParse(number.Substring(0, number.Length - 1), out var c)) return (char)c;
			if ((value.EndsWith("UL") || value.EndsWith("LU")) &&
				ulong.TryParse(number.Substring(0, number.Length - 2), out var ul)) return ul;
			if (value.EndsWith("U") && uint.TryParse(number.Substring(0, number.Length - 1), out var u)) return u;
			if (value.EndsWith("L") && long.TryParse(number.Substring(0, number.Length - 1), out var l)) return l;
			if (value.EndsWith("D") && double.TryParse(number.Substring(0, number.Length - 1), out var d)) return d;
			if (value.EndsWith("F") && float.TryParse(number.Substring(0, number.Length - 1), out var f)) return f;
			if (value.EndsWith("M") && decimal.TryParse(number.Substring(0, number.Length - 1), out var m)) return m;
			if (value.EndsWith("P") && int.TryParse(number.Substring(0, number.Length - 1), out var p))
				return new IntPtr(p);
			if ((value.EndsWith("UP") || value.EndsWith("PU")) &&
				ulong.TryParse(number.Substring(0, number.Length - 2), out var up)) return new UIntPtr(up);
			return NotParsed;
		}
	}
}
