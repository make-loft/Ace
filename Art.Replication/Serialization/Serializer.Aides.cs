﻿using System.Linq;
using System.Text;
using Art.Replication;

namespace Art.Serialization
{
    public static partial class Serializer
    {
        internal static string SnapshotToString(this object value, KeepProfile keepProfile, int indentLevel = 1) =>
            value.ToStringBeads(keepProfile, indentLevel)
                .Aggregate(new StringBuilder(), (sb, s) => sb.Append(s))
                .ToString();

        static StringBuilder b = new StringBuilder(256);
       // internal static string SnapshotToString1(this object value, KeepProfile keepProfile, int indentLevel = 1) =>
         //   keepProfile.Write(b.Clear(), value)
           //     .ToString();

        public static Snapshot CreateSnapshot(
            this object master,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => Snapshot.Create(master, replicationProfile, keepProfile);

        public static Snapshot CreateSnapshot(
            this string matrix,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => Snapshot.Create(matrix, replicationProfile, keepProfile);

        public static object Capture(this string matrix, KeepProfile keepProfile, int offset = 0) =>
            matrix.Capture(keepProfile, ref offset);
    }
}
