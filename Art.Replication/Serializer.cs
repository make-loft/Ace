using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Art.Replication
{
    public static class Serializer
    {
        public static StringBuilder Append(this StringBuilder builder, object value, KeepProfile profile, int indentLevel = 1)
        {
            switch (value)
            {
                case Map map:
                    return builder
                        .Append(profile.GetMapHead(map)) /* "{" */
                        .Append(profile, indentLevel, map)
                        .Append(profile.GetMapTail(map)); /* "}" */
                case Set set:
                    return builder
                        .Append(profile.GetSetHead(set)) /* "[" */
                        .Append(profile, indentLevel, set)
                        .Append(profile.GetSetTail(set)); /* "]" */
                default:
                    return builder.Append(profile.ToSimplex(value));
            }
        }

        public static StringBuilder Append(this StringBuilder builder, KeepProfile profile, int indentLevel, ICollection items)
        {
            var counter = 0;

            foreach (var item in items)
            {
                builder.Append(profile.GetHeadIndent(indentLevel, items, counter));

                if (items is Map && item is KeyValuePair<string, object> pair)
                {
                    profile.AppendKey(builder, pair.Key);
                    builder.Append(pair.Value, profile, indentLevel + 1);
                }
                else builder.Append(item, profile, indentLevel + 1);

                builder.Append(profile.GetTailIndent(indentLevel, items, counter++));
            }

            return builder;
        }

        public static ICollection Capture(this string data, KeepProfile profile, ref int offset, ICollection items)
        {
            while (!profile.MatchTail(data, ref offset, items is Map))
            {
                profile.SkipHeadIndent(data, ref offset);

                if (items is Map map)
                {
                    var key = profile.CaptureSimplex(data, ref offset);
                    map.Add(key, Capture(data, profile, ref offset));
                }
                else if (items is Set set) set.Add(Capture(data, profile, ref offset));

                profile.SkipTailIndent(data, ref offset);
            }

            return items;
        }

        public static object Capture(this string data, KeepProfile profile, ref int offset)
        {
            switch (profile.MatchHead(data, ref offset))
            {
                case KeepProfile.Map:
                    return data.Capture(profile, ref offset, new Map());
                case KeepProfile.Set:
                    return data.Capture(profile, ref offset, new Set());
                default:
                    var value = profile.CaptureSimplex(data, ref offset);
                    return profile.FromSimplex(value);
            }
        }
    }
}
