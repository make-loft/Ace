using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Art.Serialization.Converters;

namespace Art.Serialization
{
    public class EscapeProfile
    {
        public char EscapeChar = '\\';
        public char EscapeVerbatimChar = '\"';
        public string VerbatimPattern = "@";
        //public string HeadPattern = "\"";
        //public string TailPattern = "\"";

        public List<string> HeadPatterns = new List<string> {"\"", "<", "'"};
        public List<string> TailPatterns = new List<string> {"\"", ">", "'"};

        public Dictionary<char, string> VerbatimEscapeChars = new Dictionary<char, string> { { '\"', "\"\"" } };
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

        public Dictionary<char, char> VerbatimUnescapeChars = new Dictionary<char, char> { { '"', '\"' } };
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


        public StringBuilder AppendWithEscape(StringBuilder builder, string value, Dictionary<char, string> escapeChars,
            bool verbatim)
        {
            if (value == null) return builder;

            foreach (var c in value)
            {
                if (escapeChars.TryGetValue(c, out var s)) builder.Append(s);
                else if (!verbatim)
                {
                    int i = c;
                    if (i < 32 || 127 < i) builder.AppendFormat("\\u{0:x04}", i);
                    else builder.Append(c);
                    continue;
                }

                builder.Append(c);
            }

            return builder;
        }

        public string Convert(string value)
        {
            var useVerbatim = false; //value.Contains("\\") || value.Contains("/");
            var escapeChars = useVerbatim ? VerbatimEscapeChars : EscapeChars;
            var builder = new StringBuilder();
            if (useVerbatim) builder.Append(VerbatimPattern);
            AppendWithEscape(builder, value, escapeChars, useVerbatim);
            return builder.ToString();
        }

        public readonly List<char> NonSimplexChars = new List<char> { '{', '}', '[', ']', ',', ';', ':' }; // and all whitespaces

        public bool IsNonSimplex(char c) => char.IsWhiteSpace(c) || NonSimplexChars.Contains(c);

        public Simplex CaptureSimplex(string data, ref int offset)
        {
            var simplex = new Simplex();
            var builder = new StringBuilder();

            for (;offset < data.Length; )
            {
                var c = data[offset];
                if (char.IsWhiteSpace(c) || NonSimplexChars.Contains(c))
                {
                    if (builder.Length > 0) simplex.Add(builder.ToString());
                    //if (!char.IsWhiteSpace(c)) simplex.Add(c.ToString());
                    break;
                }

                var verbatimFlag = data.Match(VerbatimPattern, offset);
                if (verbatimFlag)
                {
                    //simplex.Add(VerbatimPattern);
                    offset += VerbatimPattern.Length;
                }

                var o = offset;
                var headPattern = HeadPatterns.FirstOrDefault(p => data.Match(p, o));
                if (headPattern != null)
                {
                    var escapeChar = verbatimFlag ? EscapeVerbatimChar : EscapeChar;
                    var unescapeStrategy = verbatimFlag ? VerbatimUnescapeChars : UnescapeChars;
                    simplex.Add(headPattern);
                    offset += headPattern.Length;
                    var index = HeadPatterns.IndexOf(headPattern);
                    var tailPattern = TailPatterns[index];
                    AppendEscapedLiteral(builder, data, ref offset, unescapeStrategy, escapeChar, tailPattern,
                        verbatimFlag);
                    simplex.Add(builder.ToString());
                    builder.Clear();

                    simplex.Add(tailPattern);
                    offset += tailPattern.Length;
                    if (offset > data.Length) throw new Exception("Unexpected end of escaped value: " + builder);
                }
                else
                {
                    builder.Append(c);
                    offset++;
                }
            }

            return simplex;
        }

        public static void AppendEscapedLiteral(
            StringBuilder builder, string data, ref int offset,
            Dictionary<char, char> unescapeStrategy, char escapeChar, string breakPattern, bool verbatim)
        {
            for (var escapeFlag = false; offset < data.Length; offset++)
            {
                var c = data[offset];
                if (escapeFlag)
                {
                    
                    if (unescapeStrategy.TryGetValue(c, out var s)) builder.Append(s);
                    else if (!verbatim && c == 'u')
                    {
                        c = (char) int.Parse(data.Substring(offset + 1, 4), NumberStyles.AllowHexSpecifier);
                        builder.Append(c);
                        offset += 4;
                    }
                    else builder.Append(c);

                    escapeFlag = false;
                    continue;
                }

                escapeFlag = c == escapeChar;
                if (escapeFlag) continue;
                if (data.Match(breakPattern, offset)) break;
                builder.Append(c);
            }
        }
    }
}
