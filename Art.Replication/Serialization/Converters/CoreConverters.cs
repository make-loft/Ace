using System.Linq;

namespace Art.Serialization.Converters
{
    public class NullConverter : Converter
    {
        public string NullLiteral = "null";
        public string[] NullLiterals = {"null", "Null", "NULL", "nil", "Nil", "NIL"};

        public override string Convert(object value) =>
            value == null ? NullLiteral : null;

        public override object Revert(string value, string typeCode) =>
            Equals(NullLiteral, value) || NullLiterals.Contains(value) ? null : NotParsed;
    }

    public class BooleanConverter : Converter
    {
        public string TrueLiteral = "true";
        public string FalseLiteral = "false";

        public override string Convert(object value) =>
            true.Equals(value)
                ? TrueLiteral
                : false.Equals(value)
                    ? FalseLiteral
                    : null;

        public override object Revert(string value, string typeCode) =>
            bool.TryParse(value, out var b) ? b : NotParsed;
    }

    public class StringSerializer : Converter
    {
        public override string Convert(object value) => value is string s ? s : null;
        public override object Revert(string value, string typeCode) => typeCode == null ? value : NotParsed;
    }
}
