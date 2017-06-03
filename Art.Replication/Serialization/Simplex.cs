using System.Collections.Generic;
using System.Linq;
using System.Text;
using Art.Serialization.Escapers;

namespace Art.Serialization
{
    public class Simplex : List<string>
    {
        public override string ToString() => this.Aggregate("", (a, b) => a + b);
        

        public static Dictionary<string, string> stringToEscape = new Dictionary<string, string>();
        public static Dictionary<string, bool> stringToVerbatim = new Dictionary<string, bool>();
        public static StringBuilder builder = new StringBuilder();
        
        public Simplex Escape(EscapeProfile escaper, int segmentIndex)
        {
            //return this;
            var segment = this[segmentIndex];
            //var useVerbatim = segment.Contains("\\") || segment.Contains("/");
       
            var useVerbatim = stringToVerbatim.TryGetValue(segment, out var v)
                ? v
                : stringToVerbatim[segment] = segment.Contains("\\") || segment.Contains("/");

            var escapeChars = useVerbatim ? escaper.VerbatimEscapeChars : escaper.EscapeChars;

            //hits = (useVerbatim ? hits.Where(h => h.Marker == "\"") : hits.Where(h => h.Marker != "\"")).ToArray();
            this[segmentIndex] = //stringToEscape.TryGetValue(segment, out var v) ? v : stringToEscape[segment] =
                //builder.Clear().Escape(segment, ProvideHits(segment), Escaper.EscapeRules, "\\").ToString();
                // this[segmentIndex] = //stringToEscape.TryGetValue(segment, out var v) ? v : stringToEscape[segment] =
                escaper.AppendWithEscape(builder.Clear(), segment, escapeChars, useVerbatim).ToString();
            if (useVerbatim) Insert(segmentIndex - 1, escaper.VerbatimPattern);
            return this;
        }

        private static List<Marker.Hit> ProvideHits(string segment)
        {
            var hits = new List<Marker.Hit>();
            Escaper.EscapeRules.ForEach(r => hits.AddRange(Marker.GetHits(segment, r.Key)));
            hits.Sort((x, y) => x.Offset - y.Offset);
            return hits;
        }
    }
}