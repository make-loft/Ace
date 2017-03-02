using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        public string NullLiteral { get; set; }
        public string TrueLiteral { get; set; }
        public string FalseLiteral { get; set; }
        public string EmptyArray { get; set; }
        public string EmptyObject { get; set; }
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

        public void SkipWhiteSpace(string data, ref int offset)
        {
            while (offset < data.Length && char.IsWhiteSpace(data[offset])) offset++;
        }

        public void SkipHeadIndent(string data, ref int offset)
        {
            while (offset < data.Length && char.IsWhiteSpace(data[offset])) offset++;
        }

        public void SkipTailIndent(string data, ref int offset)
        {
            while (offset < data.Length && (char.IsWhiteSpace(data[offset])||data[offset] == ',')) offset++;
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
            SkipWhiteSpace(data, ref offset);
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

        internal static readonly long DatetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static KeepProfile GetFormatted()
        {
            return new KeepProfile
            {
                NullLiteral = "null",
                TrueLiteral = "true",
                FalseLiteral = "false",
                EmptyArray = "[ ]",
                EmptyObject = "{ }",
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

        public string CaptureKey(string data, ref int offset)
        {
            var keyStart = offset;
            var splitterIndex = data.IndexOf(MapPairSplitter, keyStart, StringComparison.OrdinalIgnoreCase);
            var keyLength = splitterIndex - keyStart;
            var key = data.Substring(keyStart, keyLength).Trim();
            offset = splitterIndex + MapPairSplitter.Length;
            return key;
        }

        public Dictionary<char, string> EscapeChars = new Dictionary<char, string>
        {
            {'\"', "\\\""},
            {'\\', "\\\\"},
            {'/', "\\/"},
            {'\b', "\\b"},
            {'\f', "\\f"},
            {'\n', "\\n"},
            {'\r', "\\r"},
            {'\t', "\\t"},
        };

        public Dictionary<char, char> UnescapeChars = new Dictionary<char, char>
        {
            {'"', '\"'},
            {'\\', '\\'},
            {'/', '/'},
            {'b', '\b'},
            {'f', '\f'},
            {'n', '\n'},
            {'r', '\r'},
            {'t', '\t'},
        };

        public string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var builder = new StringBuilder();
            foreach (var c in value)
            {
                if (EscapeChars.TryGetValue(c, out var s)) builder.Append(s);
                else
                {
                    int i = c;
                    if (i < 32 || 127 < i) builder.AppendFormat("\\u{0:x04}", i);
                    else builder.Append(c);
                }
            }

            return builder.ToString();
        }


        public string CaptureSimplex(string data, ref int offset)
        {
            SkipWhiteSpace(data, ref offset);

            var builder = new StringBuilder();
            var escapeFlag = false;
            var quotesFlag = data[offset] == '"';
            if (quotesFlag) builder.Append(data[offset++]);

            do
            {
                var c = data[offset++];
                if (escapeFlag)
                {
                    if (UnescapeChars.TryGetValue(c, out var s))
                    {
                        builder.Append(s);
                        continue;
                    }
                    if (c == 'u')
                    {
                        c = (char) int.Parse(data.Substring(offset, 4));
                        builder.Append(c);
                        offset += 4;
                        continue;
                    }
                }

                if (!quotesFlag && (char.IsWhiteSpace(c) || c == ',')) break;
                if (c == '"' && quotesFlag && !escapeFlag)
                {
                    builder.Append(c);
                    break;
                }
                escapeFlag = c == '\\';
                if (!escapeFlag) builder.Append(c);
            } while (true);

            return builder.ToString();
        }

        public object FromSimplex(string value)
        {
            if (value.StartsWith("\"")) return value.Substring(1, value.Length -2);
            if (value == NullLiteral) return null;
            if (value == TrueLiteral) return true;
            if (value == FalseLiteral) return false;
            if (int.TryParse(value, out var l)) return l;
            if (float.TryParse(value, out var d)) return d;
            return value;
        }
        public string ToSimplex(object value)
        {
            switch (value)
            {
                case null:
                    return NullLiteral;
                case bool b:
                    return b ? TrueLiteral : FalseLiteral;
                case string s:
                    return '"' + Escape(s) + '"';
                case float n:
                    return n.ToString("G", CultureInfo.InvariantCulture);
                case double n:
                    return n.ToString("G", CultureInfo.InvariantCulture);
                case decimal n:
                    return n.ToString("G", CultureInfo.InvariantCulture);
                case Enum e:
                    return ((long) value).ToString();
                case Type t:
                    return '"' + Escape(t.AssemblyQualifiedName) + '"';
                case DateTime d:
                    return @"""\/Date(" + (d.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000 + "+" +
                           DateTimeOffset.Now.Offset.ToString("hhmm") + @")\/""";
                default:
                    return Escape(value.ToString());
                
            }
        }
    }
}
