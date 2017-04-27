using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Art.Replication.Replicators
{
    public class RegexReplicator : Replicator<Regex>
    {
        public override object Translate(object value, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null) => value is Regex regex
            ? replicationProfile.AttachType || baseType != typeof(Regex)
                ? new Map
                {
                    {replicationProfile.TypeKey, ActiveType},
                    {"Pattern", regex.ToString()},
                    {"Options", regex.Options}
                }
                : new Map
                {
                    {"Pattern", regex.ToString()},
                    {"Options", regex.Options}
                }
            : null;

        public override object Replicate(object snapshot, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null) => snapshot is Map map
            ? new Regex((string) map["Pattern"],
                (RegexOptions) Enum.Parse(typeof(RegexOptions), (string) map["Options"], true))
            : null;
    }
}
