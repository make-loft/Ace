using System;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static class ReplicationApi
    {
        public static readonly Replicator Default = new Replicator();

        public static object GetState(this object master, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<object, int>();
            var contentProvider = replicationProfile.Replicators.FirstOrDefault(p =>
                                      p.CanTranslate(master, replicationProfile, idCache, baseType)) ??
                                  Default ?? throw new Exception("Can not translate " + master);

            return contentProvider.Translate(master, replicationProfile, idCache, baseType);
        }

        public static object GetInstance(this object state, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<int, object>();
            var contentProvider = replicationProfile.Replicators.FirstOrDefault(p =>
                                      p.CanReplicate(state, replicationProfile, idCache, baseType)) ??
                                  Default ?? throw new Exception("Can not replicate " + state);

            return contentProvider.Replicate(state, replicationProfile, idCache, baseType);
        }
    }
}
