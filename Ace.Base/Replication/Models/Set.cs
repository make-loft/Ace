using System.Collections.Generic;
using Ace.Serialization;

namespace Ace.Replication.Models
{
    public class Set : List<object>, IModel<object>
    {
        public Set() { }
        public Set(IEnumerable<object> collection) : base(collection) { }
    }
}