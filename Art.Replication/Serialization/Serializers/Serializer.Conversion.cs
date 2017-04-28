using System.Collections;
using System.Collections.Generic;

namespace Art.Replication
{
    public static partial class Serializer
    {
        public static IEnumerable<string> ToStringBeads(this object value, KeepProfile keepProfile, int indentLevel = 1)
        {
            switch (value)
            {
                case Map map:
                    yield return "/*" + map.Count + "*/ ";
                    yield return keepProfile.GetHead(map); /* "{" */
                    foreach (var bead in map.ConvertComplex(keepProfile, indentLevel))
                        yield return bead;
                    yield return keepProfile.GetTail(map); /* "}" */
                    yield break;
                case Set set:
                    yield return "/*" + set.Count + "*/ ";
                    yield return keepProfile.GetHead(set); /* "[" */
                    foreach (var bead in set.ConvertComplex(keepProfile, indentLevel))
                        yield return bead;
                    yield return keepProfile.GetTail(set); /* "]" */
                    yield break;
                default: /* simplex value */
                    foreach (var bead in keepProfile.SimplexConverter.Convert(value))
                        yield return bead;
                    yield break;
            }
        }

        private static IEnumerable<string> ConvertComplex(this ICollection items, KeepProfile keepProfile, int indentLevel = 1)
        {
            var counter = 0;

            foreach (var item in items)
            {
                yield return keepProfile.GetHeadIndent(indentLevel, items, counter);

                if (items is Map && item is KeyValuePair<string, object> pair)
                {
                    yield return pair.Key;
                    yield return keepProfile.MapPairSplitter;
                    foreach (var bead in pair.Value.ToStringBeads(keepProfile, indentLevel + 1))
                        yield return bead;
                }
                else
                {
                    foreach (var bead in item.ToStringBeads(keepProfile, indentLevel + 1))
                        yield return bead;
                }

                yield return keepProfile.GetTailIndent(indentLevel, items, counter++);
            }
        }
    }
}
