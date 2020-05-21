using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ace.Replication;
using Ace.Serialization;

namespace Ace
{
	public static class Aides
	{
		internal static string SnapshotToString(this object value, KeepProfile profile,
			int indentLevel = 1, StringBuilder builder = null) =>
			profile.ToStringBeads(value, indentLevel)
				.Aggregate(builder ?? new StringBuilder(256), (sb, s) => sb.Append(s))
				.ToString();

		public static Snapshot CreateSnapshot(
			this object master,
			ReplicationProfile replicationProfile = null,
			KeepProfile keepProfile = null,
			Dictionary<object, int> cache = null,
			Type baseType = null) =>
			Snapshot.Create(master, cache, replicationProfile, keepProfile, baseType);

		public static Snapshot ParseSnapshot(
			this string matrix,
			ReplicationProfile replicationProfile = null,
			KeepProfile keepProfile = null) =>
			Snapshot.Parse(matrix, replicationProfile, keepProfile);

		public static object Capture(this string matrix, KeepProfile profile, int offset = 0) =>
			profile.ReadItem(matrix, ref offset);
	}
}