using System.Collections;
using System.Collections.Generic;
using System.Text;
using Art.Replication;

namespace Art.Serialization.Serializers
{
    public class DeepSerializer : Serializer<ICollection>
    {
        public override string ConvertStrong(ICollection value)
        {
            throw new System.NotImplementedException();
        }

        public override StringBuilder FillBuilder(StringBuilder builder, object value, KeepProfile keepProfile,
            int indentLevel = 1)
        {
            builder.Append(keepProfile.GetHead(value));
            FillBuilderStrong(builder, (ICollection) value, keepProfile, indentLevel);
            builder.Append(keepProfile.GetTail(value));
            return builder;
        }

        public virtual StringBuilder FillBuilderStrong(StringBuilder builder, ICollection items, KeepProfile keepProfile, int indentLevel = 1) 
        {
            var counter = 0;

            foreach (var item in items)
            {
                builder.Append(keepProfile.GetHeadIndent(indentLevel, items, counter));

                if (items is Map && item is KeyValuePair<string, object> pair)
                    builder
                        .Append("\"")
                        .Append(pair.Key)
                        .Append("\"")
                        .Append(keepProfile.MapPairSplitter)
                        .AppendRecursive(pair.Value, keepProfile, indentLevel + 1);
                else builder.AppendRecursive(item, keepProfile, indentLevel + 1);

                builder.Append(keepProfile.GetTailIndent(indentLevel, items, counter++));
            }

            return builder;
        }
    }
}