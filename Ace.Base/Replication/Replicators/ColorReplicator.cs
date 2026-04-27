using System;
using System.Windows.Media;

using Ace.Replication.Models;

namespace Ace.Replication.Replicators;

public class ColorReplicator : ACachingReplicator<Color>
{
	public string ValueKey = "#Value";

	public override void FillMap(Map map, ref Color instance, TranslationArgs args)
		=> map.Add(ValueKey, instance.ToString());

	public override Color ActivateInstance(Map map, ReplicationArgs args, Type baseType)
		=> (Color)ColorConverter.ConvertFromString((string)map[ValueKey]);
}
