using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Art.Replication
{
    public class EscapeProfile : IConverter<string, string>
    {
        public char EscapeChar = '\\';
        public char EscapeVerbatimChar = '\"';
        public char HeadQuoteChar = '\"';
        public char TailQuoteChar = '\"';
        public char VerbatimChar = '@';

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

        public Dictionary<char, string> VerbatimEscapeChars = new Dictionary<char, string> {{'\"', "\"\""}};

        public Dictionary<char, char> VerbatimUnescapeChars = new Dictionary<char, char> {{'"', '\"'}};

        public StringBuilder AppendWithEscape(StringBuilder builder, string value, Dictionary<char, string> escapeChars,
            bool verbatim)
        {
            if (value == null) return builder;

            foreach (var c in value)
            {
                if (escapeChars.TryGetValue(c, out var s)) builder.Append(s);
                else if (verbatim) builder.Append(c);
                else
                {
                    int i = c;
                    if (i < 32 || 127 < i) builder.AppendFormat("\\u{0:x04}", i);
                    else builder.Append(c);
                }
            }

            return builder;
        }

        public string Convert(string value)
        {
            var useVerbatim = value.Contains("\\") || value.Contains("/");
            var escapeChars = useVerbatim ? VerbatimEscapeChars : EscapeChars;
            var builder = new StringBuilder();
            if (useVerbatim) builder.Append(VerbatimChar);
            builder.Append(HeadQuoteChar);
            AppendWithEscape(builder, value, escapeChars, useVerbatim);
            builder.Append(TailQuoteChar);
            return builder.ToString();
        }

        private readonly List<char> NonSimplexChars = new List<char> {'{', '}', '[', ']', ',', ';', ':'}; // and all whitespaces

        public bool IsNonSimplex(char c) => char.IsWhiteSpace(c) || NonSimplexChars.Contains(c);


        public Simplex CaptureSimplex(string data, ref int offset)
        {
            var segments = new List<string>();

            var builder = new StringBuilder();
            do
            {
                var c = data[offset++];
                if (char.IsWhiteSpace(c) || NonSimplexChars.Contains(c))
                {
                    segments.Add(builder.ToString());
                    break;
                }

                builder.Append(c);

                var verbatimFlag = c == VerbatimChar;
                var headQuoteFlag = c == HeadQuoteChar;
                if (!headQuoteFlag) continue;

                segments.Add(builder.ToString());
                builder.Clear();

                var escapeChar = verbatimFlag ? EscapeVerbatimChar : EscapeChar;
                var unescapeStrategy = verbatimFlag ? VerbatimUnescapeChars : UnescapeChars;
                AppendEscapedLiteral(builder, data, ref offset, unescapeStrategy, escapeChar, TailQuoteChar);

                segments.Add(builder.ToString());
                builder.Clear();

                builder.Append(TailQuoteChar);
                offset++;
            } while (true);

            return new Simplex(segments);
        }

        public static void AppendEscapedLiteral(
            StringBuilder builder, string data, ref int offset,
            Dictionary<char, char> unescapeStrategy, char escapeChar, char breakChar)
        {
            for (var escapeFlag = false; ; offset++)
            {
                var c = data[offset];
                if (escapeFlag)
                {
                    if (unescapeStrategy.TryGetValue(c, out var s)) builder.Append(s);
                    else if (c == 'u')
                    {
                        c = (char) int.Parse(data.Substring(offset, 4));
                        builder.Append(c);
                        offset += 4;
                    }

                    escapeFlag = false;
                    continue;
                }

                escapeFlag = c == escapeChar;
                if (escapeFlag) continue;
                if (c == breakChar) break;
                builder.Append(c);
            }
        }
    }
}
