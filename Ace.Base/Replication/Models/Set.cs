using System.Collections.Generic;

namespace Ace.Replication.Models
{
    public class Set : List<object>
    {
        public Set() { }
        public Set(IEnumerable<object> collection) : base(collection) { }
    }
}