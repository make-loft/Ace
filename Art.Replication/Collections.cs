using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public class Map : Dictionary<string, object>
    {
        public Map()
        {
        }

        public Map(IDictionary<string, object> dictionary) : base(dictionary)
        {
        }
    }

    public class Set : List<object>
    {
        public Set()
        {
        }

        public Set(IEnumerable<object> collection) : base(collection)
        {
        }
    }

    public class Simplex
    {
        public List<string> Segments = new List<string>();
        public bool IgnoreQuotes { get; set; }

        public override string ToString() => Segments.Aggregate("", (s, s1) => s + s1);
    }
}
