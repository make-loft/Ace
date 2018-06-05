using System.Collections.Generic;
using Ace.Serialization;

namespace Ace.Replication.Models
{
    public class Map : Dictionary<string, object>, IModel<KeyValuePair<string, object>>
    {
        public Map() { }
        public Map(IDictionary<string, object> dictionary) : base(dictionary) { }

        public object Capture(KeepProfile keepProfile, string data, ref int offset)
        {
            while (!keepProfile.MapBody.TryFindTail(data, ref offset)) /* "}" */
            {
                keepProfile.SkipHeadIndent(data, ref offset);

                var key = keepProfile.CaptureKey(data, ref offset);
                keepProfile.SkipMapPairSplitter(data, ref offset);
                Add(key, data.ReadItem(keepProfile, ref offset));


                keepProfile.SkipTailIndent(data, ref offset);
            }

            return this;
        }

        public IEnumerable<string> ToStringBeads(KeepProfile keepProfile, int indentLevel) =>
            ACollectionModel.ToStringBeads(this, keepProfile, indentLevel);

        public IEnumerable<string> GetSegmentBeads(KeyValuePair<string, object> pair, KeepProfile keepProfile,
            int indentLevel)
        {
            var key = pair.Key;
            yield return keepProfile.GetKeyHead(key);
            yield return key;
            yield return keepProfile.GetKeyTail(key);
            yield return keepProfile.MapPairSplitter;
            foreach (var bead in pair.Value.ToStringBeads(keepProfile, indentLevel + 1))
                yield return bead;
        }
    }
}