using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Art.Replication.Activators;
using Art.Replication.Models;
using Art.Replication.Patterns;

namespace Art.Replication
{
    public class ReplicationProfile
    {
        public string IdKey = "#Id";
        public string SetKey = "#Set";
        public string MapKey = "#Map";
        public string TypeKey = "#Type";
        public bool AttachTypeInfo = false;
        public bool AttachId = false;
        public bool SimplifySets = true;
        public bool SimplifyMaps = true;

        public readonly Dictionary<object, int> SnapshotToIdCache = new Dictionary<object, int>();
        public readonly Dictionary<int, object> IdToReplicaCache = new Dictionary<int, object>();
        public ADataProfile Schema = new ContractProfile();

        public List<IActivator> Activators = new List<IActivator>();

        public Func<MemberInfo, bool> MembersFilter = Member.CanReadWrite;

        public bool IsSimplex(object obj) =>
            obj == null || obj is string || obj.GetType().IsPrimitive ||
            obj is Type || obj is DateTime || obj is TimeSpan || obj is DateTimeOffset || obj is Guid || obj is Uri || obj is Regex;
    }
}
