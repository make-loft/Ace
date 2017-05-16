using System;
using System.Collections.Generic;
using System.Reflection;
using Art.Replication.MemberProviders;
using Art.Replication.Replicators;

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
            new CoreMemberProvider(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, Member.CanReadWrite),
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
    }
}
