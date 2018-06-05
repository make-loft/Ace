using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class RegexReplicator : ACachingReplicator<Regex>
	{
		public string PatternKey = "#c_Pattern";
		public string OptionsKey = "#c_Options";

		public override void FillMap(Map map, Regex instance, ReplicationProfile replicationProfile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			map.Add(PatternKey, instance.ToString());
			map.Add(OptionsKey, instance.Options);
		}

		public override Regex ActivateInstance(Map map, ReplicationProfile replicationProfile,
			IDictionary<int, object> idCache, Type baseType = null) =>
			new Regex((string) map[PatternKey], RestoreOptions(map[OptionsKey], replicationProfile));

		private static RegexOptions RestoreOptions(object value, ReplicationProfile replicationProfile) =>
			value is RegexOptions regexOptions
				? regexOptions
				: replicationProfile.TryRestoreTypeInfoImplicitly
					? (RegexOptions) Enum.Parse(TypeOf<RegexOptions>.Raw, value.ToString(), true)
					: throw new Exception("Can not restore type info for value " + value);
	}
}
