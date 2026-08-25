using System.Globalization;

namespace Ace.Serialization.Converters;

public class NumericConverter : Converter
{
	public string IntegerNumbersFormat = null; //"G"; /* 'null' for best performance */
	public string RealNumbersFormat = "G";
	public bool AppendSyffixes = true;

	public readonly Dictionary<Type, string> TypeToSyffix = New.Dictionary
	(
		typeof(   byte).Of("B" ),
		typeof(   char).Of("C" ),
		typeof(    int).Of(""  ), /* "I" */
		typeof(   uint).Of("U" ),
		typeof(   long).Of("L" ),
		typeof(  ulong).Of("UL"),
		typeof(  float).Of("F" ),
		typeof( double).Of(""  ), /* "D" */
		typeof(decimal).Of("M" ),
		typeof( IntPtr).Of("P" ),
		typeof(UIntPtr).Of("UP")
	);

	public override string Encode(object value)
	{
		var convertedValue = value is null ? null : ToStringConvert(value);
		if (convertedValue.IsNot()) return null;
		var decimalSeparator = ActiveCulture.NumberFormat.NumberDecimalSeparator;
		if ((value is double || value is float || value is decimal)
			&& convertedValue.Contains(decimalSeparator).Not())
			convertedValue += decimalSeparator + "0";
		var suffix = AppendSyffixes && TypeToSyffix.TryGetValue(value.GetType(), out var s) ? s : null;
		return suffix.IsNullOrEmpty() ? convertedValue : convertedValue + suffix;
	}

	protected string ToStringConvert(object value) => value switch
	{
		  float n => n.ToString(RealNumbersFormat   , ActiveCulture),
		 double n => n.ToString(RealNumbersFormat   , ActiveCulture),
		decimal m => m.ToString(RealNumbersFormat   , ActiveCulture),
		    int i => i.ToString(IntegerNumbersFormat, ActiveCulture),
		   long i => i.ToString(IntegerNumbersFormat, ActiveCulture),
		  short i => i.ToString(IntegerNumbersFormat, ActiveCulture),
		   byte b => b.ToString(IntegerNumbersFormat, ActiveCulture),
		  sbyte b => b.ToString(IntegerNumbersFormat, ActiveCulture),
		   uint i => i.ToString(IntegerNumbersFormat, ActiveCulture),
		  ulong i => i.ToString(IntegerNumbersFormat, ActiveCulture),
		 ushort i => i.ToString(IntegerNumbersFormat, ActiveCulture),
		 IntPtr i => i.ToString(),
		UIntPtr i => i.ToString(),
		   char c => ((int)c).ToString(IntegerNumbersFormat, ActiveCulture),
		        _ => default
	};

	public override object Decode(string value, Type type)
		=> value.Length > 0 && char.IsDigit(value.Pick(-1))
			? DecodeWithoutKey(value, NumberStyles.Any, ActiveCulture)
			: DecodeByKey(value.ToUpper(), NumberStyles.Any, ActiveCulture)
			;

	private static object DecodeWithoutKey(string value, NumberStyles style, CultureInfo culture)
		=> value.Contains(culture.NumberFormat.NumberDecimalSeparator).Not() 
		&& int.TryParse(value, style, culture, out var i)
			? i
			: double.TryParse(value, style, culture, out var r)
				? r
				: Undefined
		;

	private static object DecodeByKey(string value, NumberStyles style, CultureInfo culture)
		=>
		value.EndsWith("B" ) &&    byte.TryParse(TrimEnd(value, 1),                 out var b ) ? b  :
		value.EndsWith("C" ) &&     int.TryParse(TrimEnd(value, 1), style, culture, out var c ) ? c  :
		value.EndsWith("UL") &&   ulong.TryParse(TrimEnd(value, 2), style, culture, out var ul) ? ul :
		value.EndsWith("LU") &&   ulong.TryParse(TrimEnd(value, 2), style, culture, out     ul) ? ul :
		value.EndsWith("U" ) &&    uint.TryParse(TrimEnd(value, 1), style, culture, out var u ) ? u  :
		value.EndsWith("L" ) &&    long.TryParse(TrimEnd(value, 1), style, culture, out var l ) ? l  :
		value.EndsWith("D" ) &&  double.TryParse(TrimEnd(value, 1), style, culture, out var d ) ? d  :
		value.EndsWith("F" ) &&   float.TryParse(TrimEnd(value, 1), style, culture, out var f ) ? f  :
		value.EndsWith("M" ) && decimal.TryParse(TrimEnd(value, 1), style, culture, out var m ) ? m  :
		value.EndsWith("P" ) &&     int.TryParse(TrimEnd(value, 1), style, culture, out var p ) ? new  IntPtr(p ) :
		value.EndsWith("UP") &&   ulong.TryParse(TrimEnd(value, 2), style, culture, out var up) ? new UIntPtr(up) :
		value.EndsWith("PU") &&   ulong.TryParse(TrimEnd(value, 2), style, culture, out     up) ? new UIntPtr(up) :
		Undefined;

	private static string TrimEnd(string value, int l) => value.Substring(0, value.Length - l);
}
