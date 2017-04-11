using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Art.Replication
{
    public static class Serializer
    {
        public static IEnumerable<string> ToStringBeads(this object value, KeepProfile profile, int indentLevel = 1)
        {
            switch (value)
            {
                case Map map:
                    yield return profile.GetHead(map); /* "{" */
                    foreach (var bead in map.ToStringBeads(profile, indentLevel))
                        yield return bead;
                    yield return profile.GetTail(map); /* "}" */
                    yield break;
                case Set set:
                    yield return profile.GetHead(set); /* "[" */
                    foreach (var bead in set.ToStringBeads(profile, indentLevel))
                        yield return bead;
                    yield return profile.GetTail(set); /* "]" */
                    yield break;
                default:
                    foreach (var bead in profile.SimplexConverter.Convert(value))
                        yield return bead;
                    yield break;
            }
        }

        private static IEnumerable<string> ToStringBeads(this ICollection items, KeepProfile profile, int indentLevel = 1)
        {
            var counter = 0;

            foreach (var item in items)
            {
                yield return profile.GetHeadIndent(indentLevel, items, counter);

                if (items is Map && item is KeyValuePair<string, object> pair)
                {
                    yield return pair.Key;
                    yield return profile.MapPairSplitter;
                    foreach (var bead in pair.Value.ToStringBeads(profile, indentLevel + 1))
                        yield return bead;
                }
                else
                {
                    foreach (var bead in item.ToStringBeads(profile, indentLevel + 1))
                        yield return bead;
                }

                yield return profile.GetTailIndent(indentLevel, items, counter++);
            }
        }

        public static StringBuilder Append(this StringBuilder builder, object value, KeepProfile profile, int indentLevel = 1)
        {
            switch (value)
            {
                case Map map:
                    return builder
                        .Append(profile.GetHead(map)) /* "{" */
                        .Append(map, profile, indentLevel)
                        .Append(profile.GetTail(map)); /* "}" */
                case Set set:
                    return builder
                        .Append(profile.GetHead(set)) /* "[" */
                        .Append(set, profile, indentLevel)
                        .Append(profile.GetTail(set)); /* "]" */
                default:
                    profile.SimplexConverter.Convert(value).ForEach(s => builder.Append(s));
                    return builder;
            }
        }

        public static StringBuilder Append(this StringBuilder builder, ICollection items, KeepProfile profile, int indentLevel = 1)
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

        private static ICollection Capture(this ICollection items, KeepProfile profile, string data, ref int offset)
        {
            while (!profile.MatchTail(data, ref offset, items is Map))
            {
                profile.SkipHeadIndent(data, ref offset);

                if (items is Map map)
                {
                    var key = profile.CaptureSimplex(data, ref offset).ToString();
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
                    return new Map().Capture(profile, data, ref offset);
                case KeepProfile.Set:
                    return new Set().Capture(profile, data, ref offset);
                default:
                    var simplex = profile.CaptureSimplex(data, ref offset);
                    return profile.SimplexConverter.Convert(simplex);
            }
        }
    }
}
