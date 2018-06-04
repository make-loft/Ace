using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ace.Serialization.Converters
{
	public class NumericConverter : Converter
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
			var convertedValue = value is null ? null : ToStringConvert(value);
			if (convertedValue == null) return null;
			var decimalSeparator = ActiveCulture.NumberFormat.NumberDecimalSeparator;
			if ((value is double || value is float || value is decimal)
			    && convertedValue.IndexOf(decimalSeparator, StringComparison.OrdinalIgnoreCase) < 0)
				convertedValue += decimalSeparator + "0";
			var suffix = AppendSyffixes && TypeToSyffix.TryGetValue(value.GetType(), out var s) ? s : null;
			return suffix == null ? convertedValue : convertedValue + suffix;
		}

		protected string ToStringConvert(object value) =>
			value.Match
			(
				(int i) => i.ToString(IntegerNumbersFormat, ActiveCulture),
				(long i) => i.ToString(IntegerNumbersFormat, ActiveCulture),
				(short i) => i.ToString(IntegerNumbersFormat, ActiveCulture),

				(float n) => n.ToString(RealNumbersFormat, ActiveCulture),
				(double n) => n.ToString(RealNumbersFormat, ActiveCulture),
				(decimal m) => m.ToString(RealNumbersFormat, ActiveCulture),

				(object o) => null
			) ??
			value.Match
			(
				(byte b) => b.ToString(IntegerNumbersFormat, ActiveCulture),
				(char c) => ((int) c).ToString(IntegerNumbersFormat, ActiveCulture),
				(sbyte b) => b.ToString(IntegerNumbersFormat, ActiveCulture),

				(uint i) => i.ToString(IntegerNumbersFormat, ActiveCulture),
				(ulong i) => i.ToString(IntegerNumbersFormat, ActiveCulture),
				(ushort i) => i.ToString(IntegerNumbersFormat, ActiveCulture),

				(object o) => null
			) ??
			value.Match
			(
				(IntPtr i) => i.ToString(),
				(UIntPtr i) => i.ToString(),

				(object o) => null
			);

		public override object Revert(string value, string typeCode) =>
			value.Length > 0 && char.IsDigit(value.Pick(-1))
				? RevertWithoutCode(value, NumberStyles.Any, ActiveCulture)
				: RevertByCode(value.ToUpper(), NumberStyles.Any, ActiveCulture);

		private static object RevertWithoutCode(string number, NumberStyles style, IFormatProvider provider) =>
			int.TryParse(number, style, provider, out var i) ? i :
			double.TryParse(number, style, provider, out var r) ? r :
			Undefined;

		private static object RevertByCode(string number, NumberStyles style, IFormatProvider provider) =>
			number.EndsWith("B") && byte.TryParse(TrimEnd(number, 1), out var b) ? b :
			number.EndsWith("C") && int.TryParse(TrimEnd(number, 1), style, provider, out var c) ? (char) c :
			number.EndsWith("UL") && ulong.TryParse(TrimEnd(number, 2), style, provider, out var ul) ? ul :
			number.EndsWith("LU") && ulong.TryParse(TrimEnd(number, 2), style, provider, out ul) ? ul :
			number.EndsWith("U") && uint.TryParse(TrimEnd(number, 1), style, provider, out var u) ? u :
			number.EndsWith("L") && long.TryParse(TrimEnd(number, 1), style, provider, out var l) ? l :
			number.EndsWith("D") && double.TryParse(TrimEnd(number, 1), style, provider, out var d) ? d :
			number.EndsWith("F") && float.TryParse(TrimEnd(number, 1), style, provider, out var f) ? f :
			number.EndsWith("M") && decimal.TryParse(TrimEnd(number, 1), style, provider, out var m) ? m :
			number.EndsWith("P") && int.TryParse(TrimEnd(number, 1), style, provider, out var p) ? new IntPtr(p) :
			number.EndsWith("UP") && ulong.TryParse(TrimEnd(number, 2), style, provider, out var up) ? new UIntPtr(up) :
			number.EndsWith("PU") && ulong.TryParse(TrimEnd(number, 2), style, provider, out up) ? new UIntPtr(up) :
			Undefined;

		private static string TrimEnd(string value, int l) => value.Substring(0, value.Length - l);
	}
}