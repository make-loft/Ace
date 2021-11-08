using System;
using System.Linq;
using System.Collections.Generic;
using static System.StringComparison;

namespace Ace.Serialization.Converters
{
	public class BooleanConverter : Converter
	{
		public string ActiveNoneLiteral = "~";
		public string ActiveFakeLiteral = "-";
		public string ActiveTrueLiteral = "+";
		public readonly List<string> NoneLiterals = New.List("null", "~"); // "default", "none", "no", "nil"
		public readonly List<string> FakeLiterals = New.List("false", "-"); // "fake", "not", "off", "enabled"
		public readonly List<string> TrueLiterals = New.List("true", "+"); // "truth", "yes", "on", "disabled"

		public override string Encode(object value) =>
			value.Is(default) ? ActiveNoneLiteral :
			value.Is(false) ? ActiveFakeLiteral :
			value.Is(true) ? ActiveTrueLiteral :
			null;

		public override object Decode(string value, Type type) =>
			value.Is(ActiveNoneLiteral) ? default :
			value.Is(ActiveFakeLiteral) ? false :
			value.Is(ActiveTrueLiteral) ? true :
			NoneLiterals.Contains(value, OrdinalIgnoreCase) ? default :
			FakeLiterals.Contains(value, OrdinalIgnoreCase) ? false :
			TrueLiterals.Contains(value, OrdinalIgnoreCase) ? true :
			Undefined;
	}
}