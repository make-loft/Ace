#if DESKTOP
using System;
using System.Collections.Generic;
using System.Windows.Media;

using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class ColorReplicator : ACachingReplicator<Color>
	{
		public string ValueKey = "#Value";

		public override void FillMap(Map map, ref Color instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null) =>
			map.Add(ValueKey, instance.ToString());

		public override Color ActivateInstance(Map map, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) =>
			(Color)ColorConverter.ConvertFromString((string)map[ValueKey]);
	}
}
#endif
