using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Art.Replication.Diagnostics
{
    public class ComplexData
    {
        [DataMember]
        public string Property0 { get; set; }

        [DataMember]
        public Guid Property1 { get; set; }

        [DataMember]
        public DateTime Property2 { get; set; } = DateTime.Now;
    }

    [TestClass]
    public class UnitTest1
    {
        ReplicationProfile _replicationProfile = new ReplicationProfile();
        KeepProfile keepProfile = KeepProfile.GetFormatted();

        [TestMethod]
        public void TestMethod1()
        {
            var item = new ComplexData();
            var replicator = new Replicator() {ActiveProfile = _replicationProfile};
            var snap = replicator.TranscribeSnapshotFrom(item);
            var data = new StringBuilder().Append(snap, keepProfile).ToString();
            var i = 0;
            var snap1 = data.Capture(keepProfile, ref i);
        }
    }
}
