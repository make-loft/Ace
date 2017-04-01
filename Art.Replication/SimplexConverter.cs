using System;
using System.Globalization;

namespace Art.Replication
{
    public class SimplexConverter : IConverter<Simplex, object> , IConverter<object, string>
    {
        public IConverter<string, string> EscapeConverter { get; }

        public SimplexConverter(IConverter<string, string> escapeConverter) =>
            EscapeConverter = escapeConverter;

        public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

        public string NullLiteral { get; set; } = "null";
        public string TrueLiteral { get; set; } = "true";
        public string FalseLiteral { get; set; } = "false";

        public bool AppendSyffixToNumbers;
        public bool AppendSyffixToDouble;
        public string RealNumbersFormat = "G";
        public string IntegerNumbersFormat = "G";
        internal static readonly long DatetimeMinTimeTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public object Convert(Simplex simplex)
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

            if (simplex.Segments.Count < 2) return value;
            var type = simplex.Segments[0].Replace("@", "").Replace("\"", "");
            switch (type)
            {
                case "g":
                case "guid":
                    if (Guid.TryParse(value, out var g)) return g;
                    break;
                case "dt":
                case "datetime":
                    if (DateTime.TryParse(value, out var dt)) return dt;
                    break;
            }

            return value;
        }


        public string Convert(object value)
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
                case string s:
                    return EscapeConverter.Convert(s);
                case float n:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case double n when AppendSyffixToDouble:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case double n:
                    return n.ToString(RealNumbersFormat, ActiveCulture);
                case decimal m:
                    return m.ToString(RealNumbersFormat, ActiveCulture);

                case Enum e:
                    return ((long) value).ToString();
                case Type t:
                    return "@" + EscapeConverter.Convert(t.AssemblyQualifiedName);
                case Guid d:
                    return "Guid(" + EscapeConverter.Convert(d.ToString()) + ")";
                case DateTime d:
                    return "DateTime(" + EscapeConverter.Convert(d.ToString()) + ")";
                case DateTimeOffset d:
                    return "DateTimeOffset(" + EscapeConverter.Convert(d.ToString()) + ")";
                case TimeSpan d:
                    return "TimeSpan(" + EscapeConverter.Convert(d.ToString()) + ")";
                    //return @"""\/Date(" + (d.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000 + "+" +
                    //       DateTimeOffset.Now.Offset.ToString("hhmm") + @")\/""";
                default:
                    return value.ToString();

            }
        }
    }
}
