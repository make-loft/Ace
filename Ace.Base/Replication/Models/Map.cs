using System.Collections.Generic;
using Ace.Serialization;

namespace Ace.Replication.Models
{
    public class Map : Dictionary<string, object>, IModel<KeyValuePair<string, object>>
    {
        public Map() { }
        public Map(IDictionary<string, object> dictionary) : base(dictionary) { }
    }
}