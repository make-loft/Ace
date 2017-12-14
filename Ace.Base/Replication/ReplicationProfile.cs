using System;
using System.Collections.Generic;
using System.Linq;
using Ace.Comparers;
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
        public bool AttachType = true;
        public bool AttachId = true;
        public bool SimplifySets = false;
        public bool SimplifyMaps = false;
        
        public IEqualityComparer<object> Comparer = ReferenceComparer<object>.Default;

        public List<MemberProvider> MemberProviders = new List<MemberProvider>
        {
            new CoreMemberProviderForKeyValuePair(),
            //new CoreMemberProvider(BindingFlags.Public | BindingFlags.Instance, m => m.CanReadWrite() && !(m is EventInfo)),
            new ContractMemberProvider(
                Adapters.BindingFlags.NonPublic | Adapters.BindingFlags.Public | Adapters.BindingFlags.Instance,
                Member.CanReadWrite),
        };

        public List<Replicator> Replicators = new List<Replicator>
        {
            new CoreReplicator(),
            new CoreReplicator<Type>(),
            new CoreReplicator<Guid>(),
            new CoreReplicator<Uri>(),
            new TimeCoreReplicator(),
            new RegexReplicator(),
            new StringBuilderReplicator(),
            /* recomended position for cusom replicators */
            new DeepReplicator()
        };

        public bool TryRestoreTypeInfoImplicitly = true;
        public List<Converter> ImplicitConverters = new List<Converter> {new ComplexConverter()};
        
        public object Replicate(object graph, IDictionary<int, object> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<int, object>();
            var replicator = Replicators.FirstOrDefault(i => i.CanReplicate(graph, this, idCache, baseType)) ??
                             throw new Exception("Can not replicate " + graph);
            return replicator.Replicate(graph, this, idCache, baseType);
        }
        
        public object Translate(object graph, IDictionary<object, int> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<object, int>(Comparer);
            var replicator = Replicators.FirstOrDefault(i => i.CanTranslate(graph, this, idCache, baseType)) ??
                             throw new Exception("Can not translate " + graph);
            return replicator.Translate(graph, this, idCache, baseType);
        }
    }
}