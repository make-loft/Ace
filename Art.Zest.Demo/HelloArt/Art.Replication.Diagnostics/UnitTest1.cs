using System;
using System.Globalization;
using System.Linq;
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
        public DateTime Local { get; set; } = DateTime.Now.ToLocalTime();

        [DataMember]
        public DateTime Universal { get; set; } = DateTime.Now.ToUniversalTime();

        [DataMember]
        public TimeSpan Property3 { get; set; } = TimeSpan.MinValue;


        [DataMember]
        public DateTimeOffset Property4 { get; set; } = DateTimeOffset.MinValue;

        [DataMember]
        public Uri Property5 { get; set; } = new Uri("http://makeloft.xyz");

        [DataMember]
        public object[] Objects { get; set; } = {"str", 123, 23u, DateTime.Now};
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var item = new ComplexData();
            var tmp = item.ToStringState();
            var instance = tmp.ToInstance();
            instance = tmp;
            //var results = Comparizer.GetResults(snap, snap1, "").ToList();
            //results = results;
        }
    }
}
 