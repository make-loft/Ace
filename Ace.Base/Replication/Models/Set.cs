using System.Collections.Generic;
using Ace.Serialization;

namespace Ace.Replication.Models
{
    public class Set : List<object>, IModel<object>
    {
        public Set() { }
        public Set(IEnumerable<object> collection) : base(collection) { }

        public object Capture(KeepProfile keepProfile, string data, ref int offset)
        {
            while (!keepProfile.SetBody.TryFindTail(data, ref offset)) /* "]" */
            {
                keepProfile.SkipHeadIndent(data, ref offset);

                Add(data.ReadItem(keepProfile, ref offset));

                keepProfile.SkipTailIndent(data, ref offset);
            }

            return this;
        }

        public IEnumerable<string> ToStringBeads(KeepProfile keepProfile, int indentLevel) =>
            ACollectionModel.ToStringBeads(this, keepProfile, indentLevel);

        public IEnumerable<string> GetSegmentBeads(object item, KeepProfile keepProfile, int indentLevel) =>
            item.ToStringBeads(keepProfile, indentLevel + 1);
    }
}