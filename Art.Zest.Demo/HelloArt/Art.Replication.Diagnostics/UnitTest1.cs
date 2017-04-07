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

        [DataMember]
        public TimeSpan Property3 { get; set; } = TimeSpan.MinValue;


        [DataMember]
        public DateTimeOffset Property4 { get; set; } = DateTimeOffset.MinValue;

        [DataMember]
        public Uri Property5 { get; set; } = new Uri("http://makeloft.xyz");
    }

    [TestClass]
    public class UnitTest1
    {
        ReplicationProfile _replicationProfile = new ReplicationProfile();
        KeepProfile keepProfile = KeepProfile.GetFormatted();

        [TestMethod]
        public void TestMethod1()
        {
            var x = DateTime.Now;
            var a = x.ToUniversalTime().ToString();
            var b = x.ToLocalTime().ToString();
            var item = new ComplexData();
            var replicator = new Replicator {ActiveProfile = _replicationProfile};
            var snap = replicator.TranscribeSnapshotFrom(item);
            var data = new StringBuilder().Append(snap, keepProfile).ToString();
            var i = 0;
            var snap1 = data.Capture(keepProfile, ref i);
        }
    }
}
 