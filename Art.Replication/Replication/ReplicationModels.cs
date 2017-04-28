using System.Collections.Generic;

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
}
