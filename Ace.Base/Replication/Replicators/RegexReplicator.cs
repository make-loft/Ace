using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ace.Replication.Models;

namespace Ace.Replication.Replicators
{
	public class RegexReplicator : ACachingReplicator<Regex>
	{
		public string PatternKey = "#Pattern";
		public string OptionsKey = "#Options";

		public override void FillMap(Map map, ref Regex instance, ReplicationProfile profile,
			IDictionary<object, int> idCache, Type baseType = null)
		{
			map.Add(PatternKey, instance.ToString());
			map.Add(OptionsKey, instance.Options);
		}

		public override Regex ActivateInstance(Map map, ReplicationProfile profile,
			IDictionary<int, object> idCache, Type baseType = null) =>
			new((string) map[PatternKey], RestoreOptions(map[OptionsKey], profile));

		private static RegexOptions RestoreOptions(object value, ReplicationProfile profile) =>
			value is RegexOptions regexOptions
				? regexOptions
				: profile.TryRestoreTypeInfoImplicitly
					? (RegexOptions) Enum.Parse(TypeOf<RegexOptions>.Raw, value.ToString(), true)
					: throw new Exception("Can not restore type info for value " + value);
	}
}
