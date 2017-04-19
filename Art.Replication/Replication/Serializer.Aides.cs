using System;
using System.Linq;
using System.Text;

namespace Art.Replication
{
    public class Snapshot
    {
        public object MasterState { get; set; }

        public DateTime Timestamp { get; } = DateTime.Now;

        public static Snapshot Create(object master, ReplicationProfile replicationProfile)
        {
            return new Snapshot
            {
                MasterState = master.TranscribeStateFrom(replicationProfile)
            };
        }

    }

    public static partial class Serializer
    {
        public static object CreateInstance(this string data, KeepProfile keepProfile, int offset = 0) =>
            CreateSnapshot(data, keepProfile, ref offset);

        internal static string SnapshotToString(this object value, KeepProfile keepProfile, int indentLevel = 1) =>
            value.ToStringBeads(keepProfile, indentLevel)
                .Aggregate(new StringBuilder(), (sb, s) => sb.Append(s))
                .ToString();

        internal static object StringToSnapshot(this string value, KeepProfile keepProfile, int indentLevel = 1) =>
            CreateInstance(value, keepProfile ?? ActiveKeepProfile);

        public static ReplicationProfile ActiveReplicationProfile = new ReplicationProfile();
        public static KeepProfile ActiveKeepProfile = KeepProfile.GetFormatted();

        public static string ToStringState(
            this object value,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null,
            int indentLevel = 1)

        {
            var state = value.TranscribeStateFrom(replicationProfile ?? ActiveReplicationProfile);
            var result = SnapshotToString(state, keepProfile ?? ActiveKeepProfile, indentLevel);
            return result;
        }

        public static object ToInstance(
            this string value,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null,
            int indentLevel = 1)

        {
            var state = StringToSnapshot(value, keepProfile ?? ActiveKeepProfile);
            return state.TranslateReplicaFrom(replicationProfile ?? ActiveReplicationProfile);
        }
    }
}
