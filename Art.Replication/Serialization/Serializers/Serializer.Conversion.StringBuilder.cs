using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Art.Replication;

namespace Art.Serialization.Serializers
{
    public static partial class Serializer
    {
        public static StringBuilder Append(this StringBuilder builder, object value, KeepProfile keepProfile, int indentLevel = 1)
        {
            switch (value)
            {
                case Map map:
                    return builder
                        .Append(keepProfile.GetHead(map)) /* "{" */
                        .Append(map, keepProfile, indentLevel)
                        .Append(keepProfile.GetTail(map)); /* "}" */
                case Set set:
                    return builder
                        .Append(keepProfile.GetHead(set)) /* "[" */
                        .Append(set, keepProfile, indentLevel)
                        .Append(keepProfile.GetTail(set)); /* "]" */
                default: /* simplex value */
                    keepProfile.SimplexConverter.Convert(value).ForEach(s => builder.Append(s));
                    return builder;
            }
        }

        public static StringBuilder Append(this StringBuilder builder, ICollection items, KeepProfile keepProfile, int indentLevel = 1)
        {
            var counter = 0;

            foreach (var item in items)
            {
                builder.Append(keepProfile.GetHeadIndent(indentLevel, items, counter));

                if (items is Map && item is KeyValuePair<string, object> pair)
                    builder.Append(pair.Key)
                        .Append(keepProfile.MapPairSplitter)
                        .Append(pair.Value, keepProfile, indentLevel + 1);    
                else builder.Append(item, keepProfile, indentLevel + 1);

                builder.Append(keepProfile.GetTailIndent(indentLevel, items, counter++));
            }

            return builder;
        }
    }
}
