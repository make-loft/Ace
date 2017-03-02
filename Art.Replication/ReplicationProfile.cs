using System;
using System.Collections.Generic;
using Art.Replication.Models.Data;
using Art.Wiz.Patterns;

namespace Art.Replication
{
    public class ReplicationProfile
    {
        public readonly string IdKey = "#Id";
        public readonly string SetKey = "#Set";
        public readonly string MapKey = "#Map";
        public readonly string TypeKey = "#Type";
        public readonly string SetTypeKey = "#SetType";
        public bool AttachTypeInfo = true;
        public bool AttachId = true;

        public readonly Dictionary<object, int> SnapshotToIdCache = new Dictionary<object, int>();
        public readonly Dictionary<int, object> IdToReplicaCache = new Dictionary<int, object>();
        public ADataProfile Schema = new GeneralProfile();

        public bool IsSimplex(object obj) =>
            obj == null || obj is string || obj is DateTime || obj.GetType().IsPrimitive || obj is Guid || obj is Uri || obj is Type;
    }
}
