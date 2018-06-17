using System.Collections.Generic;
using Ace.Replication.Models;

namespace Ace.Serialization.Serializators
{
    public class MapDeepSerializator : ASerializator<Map, KeyValuePair<string, object>>
    {        
        public override IEnumerable<string> GetSegmentBeads(KeyValuePair<string, object> pair, KeepProfile profile,
            int indentLevel)
        {
            var key = pair.Key;
            yield return profile.GetKeyHead(key);
            yield return key;
            yield return profile.GetKeyTail(key);
            yield return profile.MapPairSplitter;
            foreach (var bead in profile.ToStringBeads(pair.Value, indentLevel + 1))
                yield return bead;
        }

        public override object Capture(Map map, KeepProfile profile, string data, ref int offset)
        {
            while (!profile.MapBody.TryFindTail(data, ref offset)) /* "}" */
            {
                profile.SkipHeadIndent(data, ref offset);

                var key = profile.CaptureKey(data, ref offset);
                profile.SkipMapPairSplitter(data, ref offset);
                map.Add(key, profile.ReadItem(data, ref offset));

                profile.SkipTailIndent(data, ref offset);
            }

            return map;
        }
    }
}