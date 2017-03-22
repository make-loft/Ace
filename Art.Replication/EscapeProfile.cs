using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Art.Replication
{
    public class EscapeProfile : IConverter<string, string>
    {
        public char EscapeChar = '\\';
        public char EscapeCharVerbatim = '\"';

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

        public char VerbatimLiteral = '@';
        public string HeadQuote = "\"";
        public string TailQuote = "\"";

        public string Convert(string value)
        {
            var useVerbatim = value.Contains("\\") || value.Contains("/");
            var escapeChars = useVerbatim ? VerbatimEscapeChars : EscapeChars;
            var builder = new StringBuilder();
            if (useVerbatim) builder.Append(VerbatimLiteral);
            builder.Append(HeadQuote);
            AppendWithEscape(builder, value, escapeChars, useVerbatim);
            builder.Append(TailQuote);
            return builder.ToString();
        }

        public List<char> AllowedSimplexSymbols = new List<char> {'#', '_', '+', '-', '@', '"', '.'};

        public string CaptureSimplex(string data, ref int offset)
        {
            var builder = new StringBuilder();
            var escapeFlag = false;
            var useVerbatim = data[offset] == VerbatimLiteral;
            if (useVerbatim) offset++;
            var quotesFlag = data[offset] == '"';
            if (quotesFlag) builder.Append(data[offset++]);

            var unescapeStrategy = useVerbatim ? VerbatimUnescapeChars : UnescapeChars;
            var escapeChar = useVerbatim ? EscapeCharVerbatim : EscapeChar;

            do
            {
                var c = data[offset];
                if (escapeFlag)
                {
                    if (unescapeStrategy.TryGetValue(c, out var s))
                    {
                        escapeFlag = false;
                        builder.Append(s);
                        continue;
                    }
                    if (useVerbatim)
                    {
                        builder.Append(c);
                        break;
                    }

                    if (c == 'u')
                    {
                        c = (char) int.Parse(data.Substring(offset, 4));
                        builder.Append(c);
                        offset += 4;
                        continue;
                    }
                }

                if (!quotesFlag && !(char.IsLetterOrDigit(c) || AllowedSimplexSymbols.Contains(c))) break;
                escapeFlag = c == escapeChar;
                if (c == '"' && quotesFlag && !escapeFlag)
                {
                    builder.Append(c);
                    offset++;
                    break;
                }

                if (!escapeFlag) builder.Append(c);
            } while (offset++ < data.Length);

            return builder.ToString();
        }
    }
}
