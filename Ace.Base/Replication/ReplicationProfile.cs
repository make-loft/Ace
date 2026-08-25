using System.Reflection;

using Ace.Replication.MemberProviders;
using Ace.Replication.Replicators;
using Ace.Serialization;
using Ace.Serialization.Converters;

namespace Ace.Replication;

public class ReplicationProfile
{
	public string IdKey = "#Id";
	public string SetKey = "#Set";
	public string MapKey = "#Map";
	public string TypeKey = "#Type";
	public string SetDimensionKey = "#Dimensions";

	public bool AttachId = true;
	public bool? AttachType = default;
	public bool SimplifySets = false;
	public bool SimplifyMaps = false;
	public bool TryRestoreTypeInfoImplicitly = true;

	public readonly List<Converter> ImplicitConverters = New.List<Converter>(new SystemConverter());

	public readonly List<MemberProvider> MemberProviders = New.List<MemberProvider>
	(
		new CoreMemberProviderForKeyValuePair(),
		new ContractMemberProvider(Member.DefaultFlags, Member.CanReadWrite)
	);

	public readonly List<Replicator> Replicators = New.List<Replicator>
	(
		new CoreReplicator(),
		new CoreReplicator<Enum>(),
		new CoreReplicator<Type>(),
		new CoreReplicator<Guid>(),
		new CoreReplicator<Uri>(),
		new DelegateReplicator(),
		new TimeCoreReplicator(),
		new RegexReplicator(),
		new StringBuilderReplicator(),
#if DESKTOP
		new ColorReplicator(),
#endif
		/* recomended position for cusom replicators */
		new DeepReplicator()
	);

	public object Replicate(object graph, IDictionary<int, object> cache, Type baseType = null)
	{
		var args = new ReplicationArgs(this, cache);
		var replicator = Replicators.FirstOrDefault(i => i.CanReplicate(graph, args, baseType))
			?? throw new Exception("Can not replicate: " + graph);
		return replicator.Replicate(graph, args, baseType);
	}

	public object Translate(object graph, IDictionary<object, int> cache, Type baseType = null)
	{
		var args = new TranslationArgs(this, cache);
		var replicator = Replicators.FirstOrDefault(i => i.CanTranslate(graph, args, baseType))
			?? throw new Exception("Can not translate: " + graph);
		return replicator.Translate(graph, args, baseType);
	}

	public TBase Replicate<TBase>(object graph, IDictionary<int, object> cache)
		=> (TBase)Replicate(graph, cache, TypeOf<TBase>.Raw);

	public object Translate<TBase>(object graph, IDictionary<object, int> cache)
		=> Translate(graph, cache, TypeOf<TBase>.Raw);
}