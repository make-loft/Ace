using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Art.Replication.MemberProviders;
using Art.Replication.Replicators;
using Art.Serialization;
using Art.Serialization.Converters;

namespace Art.Replication
{
    public class ReplicationProfile
    {
        public string IdKey = "#Id";
        public string SetKey = "#Set";
        public string MapKey = "#Map";
        public string TypeKey = "#Type";
        public bool AttachType = false;
        public bool AttachId = false;
        public bool SimplifySets = true;
        public bool SimplifyMaps = true;

        public List<MemberProvider> MemberProviders = new List<MemberProvider>
        {
            new CoreMemberProviderForKeyValuePair(),
            new CoreMemberProvider(BindingFlags.Public | BindingFlags.Instance, Member.CanReadWrite),
            new ContractMemberProvider(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Member.CanReadWrite),
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
        public List<Converter> ImplicitConverters = new List<Converter>
        {
            new ComplexConverter()
        };

        public object Replicate(object graph, Dictionary<int, object> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<int, object>();
            var replicator = Replicators.FirstOrDefault(i => i.CanReplicate(graph, this, idCache, baseType)) ??
                             throw new Exception("Can not replicate " + graph);
            return replicator.Replicate(graph, this, idCache, baseType);
        }
        
        public object Translate(object graph, Dictionary<object, int> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<object, int>();
            var replicator = Replicators.FirstOrDefault(i => i.CanTranslate(graph, this, idCache, baseType)) ??
                             throw new Exception("Can not translate " + graph);
            return replicator.Translate(graph, this, idCache, baseType);
        }
    }
}
