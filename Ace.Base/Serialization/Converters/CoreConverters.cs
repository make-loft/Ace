using System.Collections.Generic;

namespace Ace.Serialization.Converters
{
	public class NullConverter : Converter
	{
		public string NullLiteral = "null";
		public readonly List<string> NullLiterals = New.List("null", "Null", "NULL", "nil", "Nil", "NIL");

		public override string Convert(object value) => value is null ? NullLiteral : null;

		public override object Revert(string value, string typeKey) =>
			value.Is(NullLiteral) || NullLiterals.Contains(value) ? null : Undefined;
	}

	public class BooleanConverter : Converter
	{
		public string TrueLiteral = "true";
		public string FalseLiteral = "false";

		public override string Convert(object value) =>
			value.Is(true) ? TrueLiteral :
			value.Is(false) ? FalseLiteral :
			null;

		public override object Revert(string value, string typeKey) => bool.TryParse(value, out var b) ? b : Undefined;
	}

	public class StringConverter : Converter
	{
		public override string Convert(object value) => value as string;
		public override object Revert(string value, string typeKey) => typeKey is null ? value : Undefined;
	}
}