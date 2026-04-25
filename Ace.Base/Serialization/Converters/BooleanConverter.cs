using System;
using System.Linq;
using System.Collections.Generic;
using static System.StringComparison;

namespace Ace.Serialization.Converters;

public class BooleanConverter : Converter
{
	public string ActiveNoneLiteral = "~";
	public string ActiveFalseLiteral = "-";
	public string ActiveTruthLiteral = "+";
	public readonly List<string> NoneLiterals = New.List("null", "~"); // "default", "none", "no", "nil"
	public readonly List<string> FalseLiterals = New.List("false", "-"); // "fake", "not", "off", "enabled"
	public readonly List<string> TruthLiterals = New.List("true", "+"); // "truth", "yes", "on", "disabled"

	public override string Encode(object value) => value switch
	{
		null => ActiveNoneLiteral,
		false => ActiveFalseLiteral,
		true => ActiveTruthLiteral,
		_ => null
	};

	public override object Decode(string value, Type type)
		=>
		value.Is(ActiveNoneLiteral) ? default :
		value.Is(ActiveFalseLiteral) ? false :
		value.Is(ActiveTruthLiteral) ? true :
		NoneLiterals.Contains(value, OrdinalIgnoreCase) ? default :
		FalseLiterals.Contains(value, OrdinalIgnoreCase) ? false :
		TruthLiterals.Contains(value, OrdinalIgnoreCase) ? true :
		Undefined;
}