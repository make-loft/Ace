using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Serialization.Serializators
{
    public class SetDeepSerializator : ASerializator<Set, object>
    {
        public override IEnumerable<string> GetSegmentBeads(object item, KeepProfile keepProfile, int indentLevel) =>
            item.ToStringBeads(keepProfile, indentLevel + 1);
        
        public override IEnumerable<string> ToStringBeads(object value, KeepProfile keepProfile, int indentLevel) =>
            ToStringBeads((Set)value, keepProfile, indentLevel);

        public IEnumerable<string> ToStringBeads(Set items, KeepProfile keepProfile, int indentLevel = 1)
        {
            if (keepProfile.AppendCountComments) yield return $"/*{items.Count}*/ ";
            yield return keepProfile.GetHead(items); /* "{" */
            foreach (var bead in ConvertComplex(items, keepProfile, indentLevel))
                yield return bead;
            yield return keepProfile.GetTail(items); /* "}" */
        }
        
        public override object Capture(Set set, KeepProfile keepProfile, string data, ref int offset)
        {
            while (!keepProfile.SetBody.TryFindTail(data, ref offset)) /* "]" */
            {
                keepProfile.SkipHeadIndent(data, ref offset);

                set.Add(data.ReadItem(keepProfile, ref offset));

                keepProfile.SkipTailIndent(data, ref offset);
            }

            return set;
        }
    }
}