using System.Collections.Generic;

namespace Ace.Replication.Models
{
    public class Map : Dictionary<string, object>
    {
        public Map() { }
        public Map(IDictionary<string, object> dictionary) : base(dictionary) { }
    }
}