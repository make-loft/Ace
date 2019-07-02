using System;
using System.Collections.Generic;

namespace Ace.Replication.Models
{
	public class Map : Dictionary<string, object>
	{
		public Map() { }
		public Map(IDictionary<string, object> dictionary) : base(dictionary) { }

		public new void Add(string key, object value)
		{
			try
			{
				base.Add(key, value);
			}
			catch (Exception exception)
			{
				throw new Exception($"{key} : {value}", exception);
			}
		}
	}
}