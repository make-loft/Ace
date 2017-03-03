using System;
using System.Collections.Generic;
using Art.Replication.Models.Data;
using Art.Wiz.Patterns;

namespace Art.Replication
{
    public class ReplicationProfile
    {
        public string IdKey = "#Id";
        public string SetKey = "#Set";
        public string MapKey = "#Map";
        public string TypeKey = "#Type";
        public bool AttachTypeInfo = true;
        public bool AttachId = true;

        public readonly Dictionary<object, int> SnapshotToIdCache = new Dictionary<object, int>();
        public readonly Dictionary<int, object> IdToReplicaCache = new Dictionary<int, object>();
        public ADataProfile Schema = new GeneralProfile();

        public bool IsSimplex(object obj) =>
            obj == null || obj is string || obj is DateTime || obj.GetType().IsPrimitive || obj is Guid || obj is Uri || obj is Type;
    }
}
