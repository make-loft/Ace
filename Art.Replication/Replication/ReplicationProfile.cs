using System;
using System.Collections.Generic;
using System.Reflection;
using Art.Replication.Models;
using Art.Replication.Patterns;
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

        public Func<MemberInfo, bool> MembersFilter = Member.CanReadWrite;

        public ADataProfile Schema = new ContractProfile();

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
