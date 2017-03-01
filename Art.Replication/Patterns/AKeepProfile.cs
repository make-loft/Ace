using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Art.Wiz.Patterns
{
    public class AKeepProfile
    {
        public string NullLiteral { get; set; }
        public string TrueLiteral { get; set; }
        public string FalseLiteral { get; set; }
        public string EmptyArray { get; set; }
        public string EmptyObject { get; set; }
        public string DictionaryEntryPattern { get; set; }
        public bool SimpleDictionaryFormat { get; set; }
        public string IndentChars { get; set; }
        public string NewLine { get; set; }
        public string Delimiter { get; set; }

        public string GetIndent(int indentLevel, string indent)
        {
            var result = string.Empty;
            for (var i = 0; i < indentLevel; i++)
            {
                result += indent;
            }

            return result;
        }

        private static void Append(
     this StringBuilder jsonBuilder, Dictionary<string, string> items,
     string keyValuePattern, string actualDelimiter, string emptyDelimiter, string indent)
        {
            var counter = 1;
            foreach (var item in items)
            {
                var delimiter = counter++ == items.Count ? emptyDelimiter : actualDelimiter;
                jsonBuilder.Append(indent);
                jsonBuilder.Append(string.Format(keyValuePattern, item.Key, item.Value));
                jsonBuilder.Append(delimiter);
            }
        }

        private static void Append(
            this StringBuilder jsonBuilder, IDictionary items, // Important! Use IDictionary instead ICollection
            JsonProfile profile, string actualDelimiter, string emptyDelimiter, string indent)
        {
            var counter = 1;
            foreach (var item in items)
            {
                var delimiter = counter++ == items.Count ? emptyDelimiter : actualDelimiter;
                jsonBuilder.Append(indent);
                jsonBuilder.Append(ToJson(item, profile));
                jsonBuilder.Append(delimiter);
            }
        }

        private static void Append(
            this StringBuilder jsonBuilder, ICollection items,
            JsonProfile profile, string actualDelimiter, string emptyDelimiter, string indent, int indentLevel)
        {
            var counter = 1;
            foreach (var item in items)
            {
                var delimiter = counter++ == items.Count ? emptyDelimiter : actualDelimiter;
                jsonBuilder.Append(indent);
                jsonBuilder.Append(ToJson(item, profile, typeof(object), indentLevel + 1));
                jsonBuilder.Append(delimiter);
            }
        }

        public static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            var jsonBuilder = new StringBuilder();
            foreach (var c in value)
            {
                switch (c)
                {
                    case '\"':
                        jsonBuilder.Append("\\\"");
                        break;
                    case '\\':
                        jsonBuilder.Append("\\\\");
                        break;
                    case '/':
                        jsonBuilder.Append("\\/");
                        break;
                    //case '\b':
                    //    jsonBuilder.Append("\\b");
                    //    break;
                    //case '\f':
                    //    jsonBuilder.Append("\\f");
                    //    break;
                    //case '\n':
                    //    jsonBuilder.Append("\\n");
                    //    break;
                    //case '\r':
                    //    jsonBuilder.Append("\\r");
                    //    break;
                    //case '\t':
                    //    jsonBuilder.Append("\\t");
                    //    break;
                    default:
                        int i = c;
                        if (i < 32 || i > 127)
                            jsonBuilder.AppendFormat("\\u{0:x04}", i);
                        else jsonBuilder.Append(c);
                        break;
                }
            }

            return jsonBuilder.ToString();
        }
    }
}
