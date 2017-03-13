using System;
using System.Collections;
using System.Globalization;
using System.Linq;
//using System.Runtime.Serialization;
using System.Text;

namespace Art.Replication
{
    public class KeepProfile
    {
        public const string Map = "Map";
        public const string Set = "Set";
        public const string Simplex = "Simplex";
        public ReplicationProfile ReplicationProfile { get; set; }
        public string NullLiteral;
        public string TrueLiteral;
        public string FalseLiteral { get; set; }

        public string MapPairSplitter { get; set; }

        //public DateTimeFormat DateTimeFormat { get; set; }
        public string IndentChars { get; set; }

        public string NewLineChars { get; set; }
        public string Delimiter { get; set; }
        public string MapHead { get; set; } = "{";
        public string MapTail { get; set; } = "}";
        public string SetHead { get; set; } = "[";
        public string SetTail { get; set; } = "]";
        public bool UseTailDelimiter { get; set; }

        public string GetMapHead(Map map) => MapHead;

        public string GetMapTail(Map map) => MapTail;

        public string GetSetHead(Set set) => SetHead;

        public string GetSetTail(Set set) => SetTail;

        public string GetHeadIndent(int indentLevel, ICollection items, int index)
        {
            var indent = string.Empty;
            for (var i = 0; i < indentLevel; i++)
            {
                indent += IndentChars;
            }

            return NewLineChars + indent;
        }

        public string GetTailIndent(int indentLevel, ICollection items, int index)
        {
            return items.Count == ++index && !UseTailDelimiter
                ? GetHeadIndent(indentLevel - 1, items, index)
                : Delimiter;
        }

        public void MoveToSimplex(string data, ref int offset)
        {
            while (offset < data.Length && !(char.IsLetterOrDigit(data[offset]) ||
                EscapeProfile.AllowedSimplexSymbols.Contains(data[offset]))) offset++;
        }

        public void MoveToItem(string data, ref int offset)
        {
            while (offset < data.Length && !IsItem(data[offset])) offset++;
        }

        public void SkipWhiteSpace(string data, ref int offset)
        {
            while (offset < data.Length && char.IsWhiteSpace(data[offset])) offset++;
        }

        public bool IsItem(char c) => char.IsLetterOrDigit(c) || EscapeProfile.AllowedSimplexSymbols.Contains(c) || c == '{' || c == '[';

        public void SkipHeadIndent(string data, ref int offset)
        {
            while (offset < data.Length && char.IsWhiteSpace(data[offset])) offset++;
        }

        public void SkipTailIndent(string data, ref int offset)
        {
            while (offset < data.Length && 
                (char.IsWhiteSpace(data[offset]) || 
                data[offset] == ',' || data[offset] == ';' || data[offset] == '.')) offset++;
        }

        public bool MatchMapHead(string data, ref int offset) => Match(data, ref offset, MapHead);

        public bool MatchMapTail(string data, ref int offset) => Match(data, ref offset, MapTail);

        public bool MatchSetHead(string data, ref int offset) => Match(data, ref offset, SetHead);

        public bool MatchSetTail(string data, ref int offset) => Match(data, ref offset, SetTail);

        public bool Match(string data, ref int offset, string pattern)
        {
            var isMatched = data.Substring(offset, pattern.Length) == pattern;
            offset = isMatched ? offset + MapTail.Length : offset;
            return isMatched;
        }

        public string MatchHead(string data, ref int offset)
        {
            MoveToItem(data, ref offset);
            return MatchMapHead(data, ref offset)
                ? Map
                : MatchSetHead(data, ref offset)
                    ? Set
                    : Simplex;
        }

        public string MatchTail(string data, ref int offset)
        {
            SkipWhiteSpace(data, ref offset);
            return MatchMapTail(data, ref offset)
                ? Map
                : MatchSetTail(data, ref offset)
                    ? Set
                    : Simplex;
        }

        public bool MatchTail(string data, ref int offset, bool isMap)
        {
            SkipWhiteSpace(data, ref offset);
            return isMap ? MatchMapTail(data, ref offset) : MatchSetTail(data, ref offset);
        }

        internal static readonly long DatetimeMinTimeTicks =
            (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static KeepProfile GetFormatted()
        {
            return new KeepProfile
            {
                NullLiteral = "null",
                TrueLiteral = "true",
                FalseLiteral = "false",
                MapPairSplitter = ": ",
                IndentChars = "  ",
                NewLineChars = Environment.NewLine,
                Delimiter = ","
            };
        }

        public void AppendKey(StringBuilder builder, string key)
        {
            builder.Append(key);
            builder.Append(MapPairSplitter);
        }

        public bool AppendSyffixToNumbers;

        public bool AppendSyffixToDouble;

        public string RealNumbersFormat = "G";

        public string IntegerNumbersFormat = "G";

        public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

        public EscapeProfile EscapeProfile = new EscapeProfile();

        public string CaptureSimplex(string data, ref int offset)
        {
            MoveToSimplex(data, ref offset);
            return EscapeProfile.CaptureSimplex(data, ref offset);
        }

        public object FromSimplex(string value)
        {
            if (value.StartsWith("\"")) return value.Substring(1, value.Length - 2);
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

            return value;
        }

        public string ToSimplex(object value)
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
                    return EscapeProfile.Escape(s);
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
                    return EscapeProfile.Escape(t.AssemblyQualifiedName);
                case DateTime d:
                    return @"""\/Date(" + (d.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000 + "+" +
                           DateTimeOffset.Now.Offset.ToString("hhmm") + @")\/""";
                default:
                    return value.ToString();

            }
        }
    }
}
