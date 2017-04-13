using System;
using System.Globalization;
using System.Text;

namespace Art.Replication
{
    public class SimplexConverter : IConverter<Simplex, object>, IConverter<object, Simplex>
    {
        public EscapeProfile EscapeConverter { get; }

        public SimplexConverter(EscapeProfile escapeConverter) =>
            EscapeConverter = escapeConverter;

        public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

        public string NullLiteral { get; set; } = "null";
        public string TrueLiteral { get; set; } = "true";
        public string FalseLiteral { get; set; } = "false";

        public char HeadQuoteChar = '\"';
        public char TailQuoteChar = '\"';

        public bool AppendSyffixToNumbers;
        public bool AppendSyffixToDouble;
        public string RealNumbersFormat = "G";
        public string IntegerNumbersFormat = "G";

        public Simplex Convert(object value, params object[] args)
        {
            var segment = ConvertPrimitive(value);
            if (segment != null)
                return AppendSyffixToNumbers ? new Simplex { segment, GetNumberSuffix(value) } : new Simplex { segment };

            segment = ConvertForEscape(value);
            var useVerbatim = segment.Contains("\\") || segment.Contains("/");
            var escapeChars = useVerbatim ? EscapeConverter.VerbatimEscapeChars : EscapeConverter.EscapeChars;
            EscapeConverter.AppendWithEscape(new StringBuilder(), segment, escapeChars, useVerbatim);

            var type = value.GetType();
            var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });

            return parseMethod != null || value is Uri
                ? new Simplex { "<", type.Name, ">", HeadQuoteChar.ToString(), segment, TailQuoteChar.ToString() }
                : new Simplex { HeadQuoteChar.ToString(), segment, TailQuoteChar.ToString() };
        }

        public object Convert(Simplex simplex, params object[] args)
        {
            var value = simplex.Count == 1 ? simplex[0] : simplex[1];
            if (value == NullLiteral) return null;
            if (value == TrueLiteral) return true;
            if (value == FalseLiteral) return false;

            if (int.TryParse(value, NumberStyles.Any, ActiveCulture, out var i)) return i;
            if (double.TryParse(value, NumberStyles.Any, ActiveCulture, out var r)) return r;

            var number = value.ToUpper();
            if ((value.EndsWith("UL") || value.EndsWith("LU")) && ulong.TryParse(number, out var ul)) return ul;
            if (value.EndsWith("U") && uint.TryParse(number, out var u)) return u;
            if (value.EndsWith("D") && uint.TryParse(number, out var d)) return d;
            if (value.EndsWith("F") && uint.TryParse(number, out var f)) return f;
            if (value.EndsWith("M") && uint.TryParse(number, out var m)) return m;

            if (simplex.Count < 2 || simplex[0].Length == 1) return value;
            var typeName = simplex[0].Replace("@", "").Replace("\"", "").Replace("<", "").Replace(">", "");
            if (typeName == "Uri") return new Uri(simplex[1]);

            if (typeName == "DateTime")
                return simplex[1].EndsWith("Z")
                    ? DateTime.Parse(simplex[1], ActiveCulture, DateTimeStyles.AdjustToUniversal)
                    : DateTime.Parse(simplex[1], ActiveCulture);

            var type = Type.GetType("System." + typeName);
            var parseMethod = type?.GetMethod("Parse", new[] {typeof(string)});
            return parseMethod?.Invoke(null, new object[] {simplex[1]});
        }

        public string ConvertPrimitive(object value)
        {
            switch (value)
            {
                case null:
                    return NullLiteral;
                case bool b:
                    return b ? TrueLiteral : FalseLiteral;
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
                default:
                    return null;
            }
        }

        public string GetNumberSuffix(object value)
        {
            switch (value)
            {
                case int _:
                    return null;
                case uint _:
                    return "U";
                case long _:
                    return "L";
                case ulong _:
                    return "UL";
                case float _:
                    return "F";
                case double _ when AppendSyffixToDouble:
                    return "D";
                case double _:
                    return null;
                case decimal _:
                    return "M";
                default:
                    return null;
            }
        }

        public string ConvertForEscape(object value)
        {
            switch (value)
            {
                case string s:
                    return s;
                case Enum e:
                    return e.ToString();
                case Type t:
                    return t.AssemblyQualifiedName;
                case Uri u:
                    return u.ToString();
                case Guid d:
                    return d.ToString("D");
                case DateTime d:
                    return d.ToString("O", ActiveCulture);
                case DateTimeOffset d:
                    return d.ToString("O", ActiveCulture);
                case TimeSpan d:
                    return d.ToString("G", ActiveCulture);
                default:
                    return value.ToString();
            }
        }
    }
}
