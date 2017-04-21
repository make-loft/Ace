using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static partial class Replicator
    {
        public static object GetState(this object master, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache = null, Type baseType = null)
        {
            var contentProvider = replicationProfile.ContentProviders.FirstOrDefault(p => p.CanApply(master)) ??
                                  ContentProvider.Default;
            idCache = idCache ?? new Dictionary<object, int>();
            return contentProvider.Provide(master, replicationProfile, idCache, baseType);
        }
    }
}
