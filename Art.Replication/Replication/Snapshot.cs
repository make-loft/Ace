using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Art.Serialization;

namespace Art.Replication
{
    public class Snapshot
    {
        public static ReplicationProfile DefaultReplicationProfile = new ReplicationProfile { AttachId = true };

        public static KeepProfile DefaultKeepProfile = KeepProfile.GetFormatted();

        public object MasterState { get; set; }

        public DateTime Timestamp { get; } = DateTime.Now;

        public ReplicationProfile ActiveReplicationProfile = new ReplicationProfile();
        public KeepProfile ActiveKeepProfile = KeepProfile.GetFormatted();

        public static Snapshot Create(
            object master,
            Dictionary<object, int> idCache,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => new Snapshot
        {
            MasterState = (replicationProfile ?? DefaultReplicationProfile).Translate(master, idCache),
            ActiveReplicationProfile = replicationProfile ?? DefaultReplicationProfile,
            ActiveKeepProfile = keepProfile ?? DefaultKeepProfile
        };

        public static Snapshot Create(
            string matrix,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => new Snapshot
        {
            MasterState = matrix.Capture(keepProfile ?? DefaultKeepProfile),
            ActiveReplicationProfile = replicationProfile ?? DefaultReplicationProfile,
            ActiveKeepProfile = keepProfile ?? DefaultKeepProfile
        };

        public override string ToString() => MasterState.SnapshotToString(ActiveKeepProfile);
        
        public string ToString(StringBuilder builder) => 
            MasterState.SnapshotToString(ActiveKeepProfile, 1, builder);

        public object CreateInstance(Dictionary<object, int> cache = null) => 
            ActiveReplicationProfile.Replicate(MasterState, cache?.ToDictionary(p => p.Value, p => p.Key));

        public T CreateInstance<T>(Dictionary<object, int> cache = null) =>
            (T)ActiveReplicationProfile.Replicate(MasterState, cache?.ToDictionary(p => p.Value, p => p.Key), typeof(T));
    }
}
