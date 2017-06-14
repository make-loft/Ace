using System.Collections.Generic;
using System.Linq;
using System.Text;
using Art.Replication;

namespace Art.Serialization
{
    public static partial class Serializer
    {
        internal static string SnapshotToString(this object value, KeepProfile keepProfile,
            int indentLevel = 1, StringBuilder builder = null) =>
            value.ToStringBeads(keepProfile, indentLevel)
                .Aggregate(builder ?? new StringBuilder(256), (sb, s) => sb.Append(s))
                .ToString();

        public static Snapshot CreateSnapshot(
            this object master,
            Dictionary<object, int> cache = null,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => Snapshot.Create(master, cache, replicationProfile, keepProfile);

        public static Snapshot ParseSnapshot(
            this string matrix,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => Snapshot.Parse(matrix, replicationProfile, keepProfile);

        public static object Capture(this string matrix, KeepProfile keepProfile, int offset = 0) =>
            matrix.Capture(keepProfile, ref offset);
    }
}
