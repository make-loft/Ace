using System;
using System.Linq;
using System.Text;

namespace Art.Replication
{
    public class Snapshot
    {
        public static ReplicationProfile DefaultReplicationProfile = new ReplicationProfile();
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
            MasterState = master.TranscribeStateFrom(replicationProfile ?? DefaultReplicationProfile),
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

        public object CreateInstance() => MasterState.TranslateReplicaFrom(ActiveReplicationProfile);

        public T CreateInstance<T>() => (T) MasterState.TranslateReplicaFrom(ActiveReplicationProfile, typeof(T));
    }

    public static partial class Serializer
    {
        internal static string SnapshotToString(this object value, KeepProfile keepProfile, int indentLevel = 1) =>
            value.ToStringBeads(keepProfile, indentLevel)
                .Aggregate(new StringBuilder(), (sb, s) => sb.Append(s))
                .ToString();

        public static Snapshot CreateSnapshot(
            this object master,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => Snapshot.Create(master, replicationProfile, keepProfile);

        public static Snapshot CreateSnapshot(
            this string matrix,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => Snapshot.Create(matrix, replicationProfile, keepProfile);

        public static object Capture(this string matrix, KeepProfile keepProfile, int offset = 0) =>
            Capture(matrix, keepProfile, ref offset);
    }
}
