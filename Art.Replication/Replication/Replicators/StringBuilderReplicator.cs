using System;
using System.Collections.Generic;
using System.Text;

namespace Art.Replication.Replicators
{
    public class StringBuilderReplicator : Replicator<StringBuilder>
    {
        public override object Translate(object value, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null) => value is StringBuilder builder
            ? replicationProfile.AttachType || baseType != ActiveType
                ? new Map
                {
                    {replicationProfile.TypeKey, ActiveType},
                    {"Value", builder.ToString()},
                    {"Capacity", builder.Capacity}
                }
                : new Map
                {
                    {"Value", builder.ToString()},
                    {"Capacity", builder.Capacity}
                }
            : null;

        public override object Replicate(object snapshot, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null) => snapshot is Map map
            ? new StringBuilder((string) map["Value"], (int) map["Capacity"])
            : null;
    }
}
