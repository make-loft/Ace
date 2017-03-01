using System.Collections.Generic;

namespace Art.Replication
{
    public class Map : Dictionary<string, object>
    {
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
}
