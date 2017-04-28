using System;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication
{
    public static class DeepOperations
    {
        public static object RecursiveTranslate(this object value, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<object, int>();
            var contentProvider = replicationProfile.Replicators.FirstOrDefault(p =>
                                      p.CanTranslate(value, replicationProfile, idCache, baseType)) ??
                                  throw new Exception("Can not translate " + value);

            return contentProvider.Translate(value, replicationProfile, idCache, baseType);
        }

        public static object RecursiveReplicate(this object value, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache = null, Type baseType = null)
        {
            idCache = idCache ?? new Dictionary<int, object>();
            var contentProvider = replicationProfile.Replicators.FirstOrDefault(p =>
                                      p.CanReplicate(value, replicationProfile, idCache, baseType)) ??
                                  throw new Exception("Can not replicate " + value);

            return contentProvider.Replicate(value, replicationProfile, idCache, baseType);
        }
    }
}
