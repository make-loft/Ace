using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Art.Comparers;
using Art.Serialization;

namespace Art.Replication
{
    public class ReconstructionCache : Dictionary<object, int>
    {
        public ReconstructionCache() : base(32, ReferenceComparer<object>.Default)
        {
        }

        public ReconstructionCache(IEqualityComparer<object> comparer) : base(32, comparer)
        {
        }
    }

    public class Snapshot
    {
        static Snapshot()
        {
            //if (DateTime.Now > new DateTime(2017, 9, 1))
            //    throw new Exception(
            //        "Trial version has been expired. Please, write to 'makeman@tut.by' for getting licence.");
        }

        public static ReplicationProfile DefaultReplicationProfile = new ReplicationProfile {AttachId = true};

        public static KeepProfile DefaultKeepProfile = KeepProfile.GetFormatted();

        public object MasterState { get; set; }

        public DateTime Timestamp { get; } = DateTime.Now;

        public ReplicationProfile ActiveReplicationProfile = new ReplicationProfile();
        public KeepProfile ActiveKeepProfile = KeepProfile.GetFormatted();

        public static Snapshot Create(
            object masterGraph,
            Dictionary<object, int> idCache,
            ReplicationProfile replicationProfile = null,
            KeepProfile keepProfile = null) => new Snapshot
        {
            MasterState = (replicationProfile ?? DefaultReplicationProfile).Translate(masterGraph, idCache),
            ActiveReplicationProfile = replicationProfile ?? DefaultReplicationProfile,
            ActiveKeepProfile = keepProfile ?? DefaultKeepProfile
        };

        public static Snapshot Parse(
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

        public object ReplicateGraph() => ActiveReplicationProfile.Replicate(MasterState);

        public TRoot ReplicateGraph<TRoot>() =>
            (TRoot) ActiveReplicationProfile.Replicate(MasterState, null, typeof(TRoot));

        public object ReconstructGraph(IDictionary<object, int> cache) =>
            ActiveReplicationProfile.Replicate(MasterState, cache?.ToDictionary(p => p.Value, p => p.Key));
    }
}
