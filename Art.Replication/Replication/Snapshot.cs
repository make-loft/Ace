using System;
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
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => new Snapshot
        {
            MasterState = (replicationProfile ?? DefaultReplicationProfile).Translate(master),
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

        public object CreateInstance() => ActiveReplicationProfile.Replicate(MasterState);

        public T CreateInstance<T>() => (T)ActiveReplicationProfile.Replicate(MasterState, null, typeof(T));
    }
}
