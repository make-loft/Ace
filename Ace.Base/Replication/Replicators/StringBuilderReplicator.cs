using System;
using System.Collections.Generic;
using System.Text;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class StringBuilderReplicator : ACachingReplicator<StringBuilder>
	{
		public string ValueKey = "#c_Value";
		public string CapacityKey = "#c_Capacity";

		public override void FillMap(Map map, ref StringBuilder instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			map.Add(ValueKey, instance.ToString());
			map.Add(CapacityKey, instance.Capacity);
		}

		public override StringBuilder ActivateInstance(Map map, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) =>
			new((string) map[ValueKey], (int) map[CapacityKey]);
	}
}
