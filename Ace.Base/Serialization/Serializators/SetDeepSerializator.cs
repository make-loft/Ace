using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Serialization.Serializators
{
    public class SetDeepSerializator : ASerializator<Set, object>
    {     
        public override IEnumerable<string> GetSegmentBeads(object item, KeepProfile keepProfile, int indentLevel) =>
            keepProfile.ToStringBeads(item, indentLevel + 1);
        
        public override object Capture(Set set, KeepProfile keepProfile, string data, ref int offset)
        {
            while (!keepProfile.SetBody.TryFindTail(data, ref offset)) /* "]" */
            {
                keepProfile.SkipHeadIndent(data, ref offset);

                set.Add(keepProfile.ReadItem(data, ref offset));

                keepProfile.SkipTailIndent(data, ref offset);
            }

            return set;
        }
    }
}