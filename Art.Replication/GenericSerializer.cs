using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Art.Wiz.Patterns;

namespace Art.Wiz
{
    public static class GenericSerializer
    {
       private static string ToObject(this object value, ADataProfile dataProfile, AKeepProfile keepProfile, Type memberType, int indentLevel)
        {
            var builder = new StringBuilder();
            var indent = keepProfile.NewLine + keepProfile.GetIndent(indentLevel, keepProfile.IndentChars);
            var emptyDelimiter = keepProfile.NewLine + keepProfile.GetIndent(indentLevel - 1, keepProfile.IndentChars);

            var dictionary = value as IDictionary;
            if (dictionary != null && keepProfile.SimpleDictionaryFormat)
            {
                if (dictionary.Count == 0) return keepProfile.EmptyObject;
                builder.Append(dictionary, keepProfile, profile.Delimiter, emptyDelimiter, indent);
            }
            else
            {
                var type = value.GetType();
                var dataMembers = dataProfile.GetDataMembers(type);
                var isEmpty = dataMembers.Count == 0;
                if (memberType != null && memberType != type)
                {
                    builder.AppendTypeInfo(keepProfile, isEmpty, emptyDelimiter, indent, type);
                    if (isEmpty) return "{" + builder + "}";
                }
                if (isEmpty) return keepProfile.EmptyObject;

                var members = dataMembers.ToDictionary(p => p.Key,
                    p => p.Value.GetValue(value).ToJson(profile, p.Value.GetMemberType(), indentLevel + 1));

                builder.Append(members, keepProfile.DictionaryEntryPattern, profile.Delimiter, emptyDelimiter, indent);
            }

            return "{" + builder + "}";
        }

        private static void AppendTypeInfo(this StringBuilder builder, AKeepProfile keepProfile, bool isEmpty, string emptyDelimiter,
            string indent, Type type)
        {
            var delimiter = isEmpty ? emptyDelimiter : keepProfile.Delimiter;
            builder.Append(indent);
            var typeName = type.FullName
                .Substring(type.Namespace == null ? 0 : type.Namespace.Length + 1).Replace("+", ".");
            builder.Append(string.Format(keepProfile.DictionaryEntryPattern, "__type",
                '"' + typeName + ":#" + type.Namespace + '"'));
            builder.Append(delimiter);
        }
    }
}
