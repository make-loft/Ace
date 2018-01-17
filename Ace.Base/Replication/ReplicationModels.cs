using System.Collections.Generic;

namespace Ace.Replication
{
	public class Set : List<object>
	{
		public Set() { }
		public Set(IEnumerable<object> collection) : base(collection) { }
	}
	
	public class Map : Dictionary<string, object>
	{
		public Map() { }
		public Map(IDictionary<string, object> dictionary) : base(dictionary) { }
	}
}