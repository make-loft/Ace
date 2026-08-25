using System.Text;

namespace Ace.Replication.Replicators;

public class StringBuilderReplicator : ACachingReplicator<StringBuilder>
{
	public string ValueKey = "#c_Value";
	public string CapacityKey = "#c_Capacity";

	public override void FillMap(Map map, ref StringBuilder instance, TranslationArgs args)
	{
		map.Add(ValueKey, instance.ToString());
		map.Add(CapacityKey, instance.Capacity);
	}

	public override StringBuilder ActivateInstance(Map map, ReplicationArgs args, Type baseType)
		=> new((string) map[ValueKey], (int) map[CapacityKey]);
}
