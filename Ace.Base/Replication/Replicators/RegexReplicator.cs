using System;
using System.Text.RegularExpressions;

using Ace.Replication.Models;

namespace Ace.Replication.Replicators;

public class RegexReplicator : ACachingReplicator<Regex>
{
	public string PatternKey = "#Pattern";
	public string OptionsKey = "#Options";

	public override void FillMap(Map map, ref Regex instance, TranslationArgs args)
	{
		map.Add(PatternKey, instance.ToString());
		map.Add(OptionsKey, instance.Options);
	}

	public override Regex ActivateInstance(Map map, ReplicationArgs args, Type baseType)
		=> new((string) map[PatternKey], RestoreOptions(map[OptionsKey], args.Profile));

	private static RegexOptions RestoreOptions(object value, ReplicationProfile profile)
		=> value is RegexOptions regexOptions
			? regexOptions
			: profile.TryRestoreTypeInfoImplicitly
				? (RegexOptions) Enum.Parse(TypeOf<RegexOptions>.Raw, value.ToString(), true)
				: throw new Exception("Can not restore type info for value: " + value)
			;
}
