using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Serialization.Serializators
{
    public class MapDeepSerializator : ASerializator<Map, KeyValuePair<string, object>>
    {        
        public override IEnumerable<string> GetSegmentBeads(KeyValuePair<string, object> pair, KeepProfile keepProfile,
            int indentLevel)
        {
            var key = pair.Key;
            yield return keepProfile.GetKeyHead(key);
            yield return key;
            yield return keepProfile.GetKeyTail(key);
            yield return keepProfile.MapPairSplitter;
            foreach (var bead in keepProfile.ToStringBeads(pair.Value, indentLevel + 1))
                yield return bead;
        }

        public override object Capture(Map map, KeepProfile keepProfile, string data, ref int offset)
        {
            while (!keepProfile.MapBody.TryFindTail(data, ref offset)) /* "}" */
            {
                keepProfile.SkipHeadIndent(data, ref offset);

                var key = keepProfile.CaptureKey(data, ref offset);
                keepProfile.SkipMapPairSplitter(data, ref offset);
                map.Add(key, keepProfile.ReadItem(data, ref offset));

                keepProfile.SkipTailIndent(data, ref offset);
            }

            return map;
        }
    }
}