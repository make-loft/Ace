using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ace.Serialization;

namespace Ace.Replication
{
	public static class Cache
	{
		public static Dictionary<int, object> NewForReplication() =>
			new Dictionary<int, object>(32);
	}

	public class Snapshot
	{
		//static Snapshot()
		//{
			//if (DateTime.Now > new DateTime(2017, 9, 1))
			//	throw new Exception(
			//		"Trial version has been expired. Please, write to 'makeman@tut.by' for getting licence.");
		//}

		public static ReplicationProfile DefaultReplicationProfile = new ReplicationProfile {AttachId = true};
		public static KeepProfile DefaultKeepProfile = KeepProfile.GetFormatted();

		public object MasterState { get; set; }
		public DateTime Timestamp { get; } = DateTime.Now;

		public ReplicationProfile ActiveReplicationProfile = new ReplicationProfile();
		public KeepProfile ActiveKeepProfile = KeepProfile.GetFormatted();

		public static Snapshot Create(
			object masterGraph,
			Dictionary<object, int> idCache = null,
			ReplicationProfile replicationProfile = null,
			KeepProfile keepProfile = null,
			Type baseType = null) => new Snapshot
		{
			MasterState = replicationProfile.Or(DefaultReplicationProfile).Translate(masterGraph, idCache.OrNew(), baseType),
			ActiveReplicationProfile = replicationProfile.Or(DefaultReplicationProfile),
			ActiveKeepProfile = keepProfile.Or(DefaultKeepProfile)
		};

		public static Snapshot Parse(
			string matrix,
			ReplicationProfile replicationProfile = null,
			KeepProfile keepProfile = null) => new Snapshot
		{
			MasterState = matrix.Capture(keepProfile.Or(DefaultKeepProfile)),
			ActiveReplicationProfile = replicationProfile.Or(DefaultReplicationProfile),
			ActiveKeepProfile = keepProfile.Or(DefaultKeepProfile)
		};

		public override string ToString() => MasterState.SnapshotToString(ActiveKeepProfile);

		public string ToString(KeepProfile profile) => MasterState.SnapshotToString(profile);

		public string ToString(StringBuilder builder) =>
			MasterState.SnapshotToString(ActiveKeepProfile, 1, builder);

		public object ReplicateGraph(Type baseType, ReplicationProfile replicationProfile = null) =>
			(replicationProfile ?? ActiveReplicationProfile).Replicate(MasterState, Cache.NewForReplication(), baseType);

		public TRoot ReplicateGraph<TRoot>(ReplicationProfile replicationProfile = null) =>
			(TRoot)(replicationProfile ?? ActiveReplicationProfile).Replicate(MasterState, Cache.NewForReplication(), TypeOf<TRoot>.Raw);

		public object ReconstructGraph(IDictionary<object, int> cache, ReplicationProfile replicationProfile = null) =>
			(replicationProfile ?? ActiveReplicationProfile).Replicate(MasterState, cache?.ToMirrorDictionary());
	}
}
