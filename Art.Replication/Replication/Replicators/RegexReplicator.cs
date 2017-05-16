using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Art.Replication.Replicators
{
    public class RegexReplicator : ACachingReplicator<Regex>
    {
        public string PatternKey = "#c_Pattern";
        public string OptionsKey = "#c_Options";

        public override void FillMap(Map map, Regex instance, ReplicationProfile replicationProfile,
            Dictionary<object, int> idCache, Type baseType = null)
        {
            map.Add(PatternKey, instance.ToString());
            map.Add(OptionsKey, instance.Options);
        }

        public override Regex ActivateInstance(Map map, ReplicationProfile replicationProfile,
            Dictionary<int, object> idCache, Type baseType = null) => 
            new Regex((string) map[PatternKey], (RegexOptions) map[OptionsKey]);
    }
}
