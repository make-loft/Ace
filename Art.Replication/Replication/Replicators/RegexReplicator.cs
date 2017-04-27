using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Art.Replication.Replicators
{
    public class RegexReplicator : Replicator<Regex>
    {
        private const string PatternKey = "#c_Pattern";
        private const string OptionsKey = "#c_Options";

        public override object Translate(object value, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null) => value is Regex regex
            ? replicationProfile.AttachType || baseType != typeof(Regex)
                ? new Map
                {
                    {replicationProfile.TypeKey, ActiveType},
                    {PatternKey, regex.ToString()},
                    {OptionsKey, regex.Options}
                }
                : new Map
                {
                    {PatternKey, regex.ToString()},
                    {OptionsKey, regex.Options}
                }
            : null;

        public override object Replicate(object snapshot, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null) => snapshot is Map map
            ? new Regex((string) map[PatternKey], (RegexOptions) map[OptionsKey])
        : null;
    }
}
