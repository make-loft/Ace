using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Art.Replication
{
    public class SimplexConverter : IConverter<Simplex, object> , IConverter<object, Simplex>
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
        internal static readonly long DatetimeMinTimeTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public object Convert(Simplex simplex, params object[] args)
        {
            var value = simplex.Segments.Count == 1 ? simplex.Segments[0] : simplex.Segments[1];
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
            
            if (simplex.Segments.Count < 2 || simplex.Segments[0].Length == 1) return value;
            var typeName = simplex.Segments[0].Replace("@", "").Replace("\"", "").Replace("<", "").Replace(">", "");
            var type = Type.GetType("System." + typeName);
            var parseMethod = type?.GetMethod("Parse", new[] {typeof(string)});
            return parseMethod?.Invoke(null, new object[] {simplex.Segments[1]});
        }

        public string ConvertUnescaped(object value)
        {
            if (AppendSyffixToNumbers)
            {
                switch (value)
                {
                    case int i:
                        return i.ToString(IntegerNumbersFormat);
                    case uint i:
                        return i.ToString(IntegerNumbersFormat) + "U";
                    case long i:
                        return i.ToString(IntegerNumbersFormat) + "L";
                    case ulong i:
                        return i.ToString(IntegerNumbersFormat) + "UL";
                    case float n:
                        return n.ToString(RealNumbersFormat, ActiveCulture) + "F";
                    case double n when AppendSyffixToDouble:
                        return n.ToString(RealNumbersFormat, ActiveCulture) + "D";
                    case double n:
                        return n.ToString(RealNumbersFormat, ActiveCulture);
                    case decimal m:
                        return m.ToString(RealNumbersFormat, ActiveCulture) + "M";
                }
            }

            switch (value)
            {
                case null:
                    return NullLiteral;
                case bool b:
                    return b ? TrueLiteral : FalseLiteral;
                case float n:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case double n when AppendSyffixToDouble:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case double n:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case decimal m:
                    return m.ToString(RealNumbersFormat, ActiveCulture);
            }

            return null;
        }

        public string ConvertForEscape(object value)
        {
            switch (value)
            {
                case string s:
                    return s;
                case Enum e:
                    return ((long)value).ToString();
                case Type t:
                    return  t.AssemblyQualifiedName;
                //case Uri u:
                //    return u.ToString();
                //case Guid d:
                //    return d.ToString();
                //case DateTime d:
                //    return d.ToString();
                //case DateTimeOffset d:
                //    return d.ToString();
                //case TimeSpan d:
                //    return d.ToString();
                //return @"""\/Date(" + (d.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000 + "+" +
                //       DateTimeOffset.Now.Offset.ToString("hhmm") + @")\/""";
                default:
                    return value.ToString();

            }
        }

        public Simplex Convert(object value, params object[] args)
        {
            var segment = ConvertUnescaped(value);
            if (segment != null) return new Simplex {Segments = {segment}};

            segment = ConvertForEscape(value);
            var useVerbatim = segment.Contains("\\") || segment.Contains("/");
            var escapeChars = useVerbatim ? EscapeConverter.VerbatimEscapeChars : EscapeConverter.EscapeChars;
            EscapeConverter.AppendWithEscape(new StringBuilder(), segment, escapeChars, useVerbatim);

            var type = value.GetType();
            var parseMethod = type.GetMethod("Parse", new []{typeof(string)});

            return parseMethod != null
                ? new Simplex {Segments = {"<", type.Name, ">", HeadQuoteChar.ToString(), segment, TailQuoteChar.ToString()}}
                : new Simplex {Segments = {HeadQuoteChar.ToString(), segment, TailQuoteChar.ToString()}};
        }
    }
}
