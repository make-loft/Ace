using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static partial class Replicator
    {
        public static object GetInstance(this object state, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache = null, Type baseType = null)
        {
            var contentProvider = replicationProfile.ContentProviders.FirstOrDefault(p => p.CanApply(state)) ??
                                  ContentProvider.Default;
            idCache = idCache ?? new Dictionary<int, object>();
            return contentProvider.ProvideBack(state, replicationProfile, idCache, baseType);
        }
    }
}
