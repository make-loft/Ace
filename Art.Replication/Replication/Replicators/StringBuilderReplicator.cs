﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Art.Replication.Replicators
{
    public class StringBuilderReplicator : ACachingReplicator<StringBuilder>
    {
        public string ValueKey = "#c_Value";
        public string CapacityKey = "#c_Capacity";

        public override void FillMap(Map map, StringBuilder instance, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null)
        {
            map.Add(ValueKey, instance.ToString());
            map.Add(CapacityKey, instance.Capacity);
        }

        public override StringBuilder ActivateInstance(Map map, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null) =>
            new StringBuilder((string) map[ValueKey], (int) map[CapacityKey]);
    }
}
