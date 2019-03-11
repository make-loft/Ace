using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ace.Replication.MemberProviders;
using Ace.Replication.Replicators;
using Ace.Serialization;
using Ace.Serialization.Converters;

namespace Ace.Replication
{
	public class ReplicationProfile
	{
		public string IdKey = "#Id";
		public string SetKey = "#Set";
		public string MapKey = "#Map";
		public string TypeKey = "#Type";
		public string SetDimensionKey = "#Dimensions";

		public bool AttachId = true;
		public bool AttachType = true;
		public bool SimplifySets = false;
		public bool SimplifyMaps = false;
		public bool TryRestoreTypeInfoImplicitly = true;

		public readonly List<Converter> ImplicitConverters = New.List<Converter>(new ComplexConverter());

		public static readonly Adapters.BindingFlags DefaultFlags =
			Adapters.BindingFlags.NonPublic | Adapters.BindingFlags.Public | Adapters.BindingFlags.Instance;

		public readonly List<MemberProvider> MemberProviders = New.List<MemberProvider>
		(
			new CoreMemberProviderForKeyValuePair(),
			//new CoreMemberProvider(BindingFlags.Public | BindingFlags.Instance, m => m.CanReadWrite() && !(m is EventInfo)),
			new ContractMemberProvider(DefaultFlags, Member.CanReadWrite)
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
			/* recomended position for cusom replicators */
			new DeepReplicator()
		);

		public object Replicate(object graph, IDictionary<int, object> idCache, Type baseType = null)
		{
			var replicator = Replicators.FirstOrDefault(i => i.CanReplicate(graph, this, idCache, baseType)) ??
							 throw new Exception("Can not replicate " + graph);
			return replicator.Replicate(graph, this, idCache, baseType);
		}

		public object Translate(object graph, IDictionary<object, int> idCache, Type baseType = null)
		{
			var replicator = Replicators.FirstOrDefault(i => i.CanTranslate(graph, this, idCache, baseType)) ??
							 throw new Exception("Can not translate " + graph);
			return replicator.Translate(graph, this, idCache, baseType);
		}

		public TBase Replicate<TBase>(object graph, IDictionary<int, object> idCache) =>
			(TBase)Replicate(graph, idCache, TypeOf<TBase>.Raw);

		public object Translate<TBase>(object graph, IDictionary<object, int> idCache) =>
			Translate(graph, idCache, TypeOf<TBase>.Raw);
	}
}