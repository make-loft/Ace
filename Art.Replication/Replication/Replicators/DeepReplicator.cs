using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Art.Replication.Replicators
{
    public class DeepReplicator : ACachingReplicator<object>
    {
        public override bool CanReplicate(object value, ReplicationProfile replicationProfile, Dictionary<int, object> idCache, Type baseType = null)
        {
            return true;
        }

        public override void FillMap(Map snapshot, object instance, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null)
        {
            var type = instance.GetType();

            if (instance is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var items = new Map(map.Cast<DictionaryEntry>()
                    .ToDictionary(p => (string) p.Key, p => p.Value.RecursiveTranslate(replicationProfile, idCache)));
                snapshot.Add(replicationProfile.MapKey, items);
            }
            else if (instance is IList set)
            {
                var items = new Set(set.Cast<object>().Select(i => i.RecursiveTranslate(replicationProfile, idCache)));
                snapshot.Add(replicationProfile.SetKey, items);
            }

            var memberProvider = replicationProfile.MemberProviders.First(p => p.CanApply(type));
            var members = memberProvider.GetDataMembers(type);
            members.ForEach(m => snapshot.Add(memberProvider.GetDataKey(m),
                m.GetValue(instance).RecursiveTranslate(replicationProfile, idCache, m.GetMemberType())));
        }

        public override void FillInstance(Map snapshot, object replica, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache,
            Type baseType = null)
        {
            var type = replica.GetType();
            if (replica is IDictionary map && type.IsGenericDictionaryWithKey<string>())
            {
                var items = (IDictionary) snapshot[replicationProfile.MapKey];
                items.Cast<DictionaryEntry>().ForEach(p => map.Add(p.Key, p.Value));
            }
            else if (replica is IList set)
            {
                var items = (Set) snapshot[replicationProfile.SetKey];
                if (replica is Array array)
                {
                    var source = items.Select(i => i.RecursiveReplicate(replicationProfile, idCache)).ToArray();
                    Array.Copy(source, array, items.Count); /* array [replica] is cached instance */
                }
                else items.ForEach(i => set.Add(i.RecursiveReplicate(replicationProfile, idCache)));
            }

            var memberProvider = replicationProfile.MemberProviders.First(p => p.CanApply(type));
            var members = memberProvider.GetDataMembers(type);
            members.ForEach(m => m.SetValueIfCanWrite(replica, /* should enumerate items at read-only members too */
                snapshot[memberProvider.GetDataKey(m)]
                    .RecursiveReplicate(replicationProfile, idCache, m.GetMemberType())));
        }

        public override object ActivateInstance(Map snapshot, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null)
        {
            var type = snapshot.TryGetValue(replicationProfile.TypeKey, out var typeName)
                ? Type.GetType(typeName.ToString(), true)
                : baseType ?? throw new Exception("Missed type info. Can not restore implicitly.");

            return type.IsArray
                ? Array.CreateInstance(type.GetElementType(), ((Set) snapshot[replicationProfile.SetKey]).Count)
                : Activator.CreateInstance(type);
        }
    }
}
