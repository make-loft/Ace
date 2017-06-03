using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Art.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Art.Replication.Diagnostics
{
    public static class Program
    {
        public static void Main()
        {
            var t = new EscapeTests();
            //t.TestSkipGeneralEscaper();
            var ut = new UnitTest1();
            ut.TestMethod1();

        }
    }

    

    [DataContract]
    [Serializable]
    public class ComplexData
    {
        [DataMember]
        public int? P = 7;
        
        [DataMember]
        public byte B = 44;

        [DataMember]
        public char C = 'c';

        [DataMember]
        public string Property0 { get; set; } = "абв";

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
        public Regex Regex0 { get; set; } = new Regex("abc", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [DataMember]
        public object[] Objects { get; set; } =
            {"str", null, 123, 23u, 123L, 345f, 456.12d, DateTime.Now, DateTime.Now.ToString(), Guid.NewGuid(), new int?(9)};

        [DataMember]
        public int[] Ints = {1, 2, 3, 4, 5, 6, 7, 8, 7};

        [DataMember] public object[] Test;
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod2()
        {

            var masterItem = new ComplexData();
            masterItem.Test = new object[] { masterItem, masterItem.Objects };
            var masterSnaphot = masterItem.CreateSnapshot();
            var replicationMatrix = masterSnaphot.ToString();
            var clonedSnapshot = replicationMatrix.CreateSnapshot();
            var clonedItem = clonedSnapshot.CreateInstance<ComplexData>();
            var lastSnapshot = clonedItem.CreateSnapshot();
            var results = masterSnaphot.GetResults(lastSnapshot, "").ToList();
            results = results;
        }

        public void Test(int i, int yi, int io, int ikj){}
        [TestMethod]
        public void TestMethod1()
        {
            //var a = new Regex("a");
            //var b = new Regex("a");
            //var t = a == b;

            var masterItem = new ComplexData();
            //masterItem.Test = new object[] {masterItem, masterItem.Objects};
            var sw= new Stopwatch();
            sw.Reset();
            sw.Start();
            for (int i = 0; i < 0000; i++)
            {
                //Test(i, i, i, i);
                var dc = masterItem.DeepClone();
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new StringEnumConverter());
            //var json = JsonConvert.SerializeObject(collection, settings);
            sw.Reset();
            sw.Start();
            var cc = Newtonsoft.Json.JsonConvert.SerializeObject(masterItem, settings);
            for (int i = 0; i < 20000; i++)
            {
                //var x = Newtonsoft.Json.JsonConvert.SerializeObject(masterItem, settings);
                var y = Newtonsoft.Json.JsonConvert.DeserializeObject<ComplexData>(cc);
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);
            sw.Reset();
            sw.Start();

            //Snapshot.DefaultKeepProfile.SimplexConverter.AppendSyffixes = false;
            Snapshot.DefaultKeepProfile.SimplexConverter.AppendTypeInfo = false;
            Snapshot.DefaultReplicationProfile.AttachId = false;
            var xx = masterItem.CreateSnapshot().ToString();
            var sn = masterItem.CreateSnapshot();
            for (int i = 0; i < 20000; i++)
            {
                //var ert = masterItem.CreateSnapshot().ToString();
                var dc = xx.CreateSnapshot().CreateInstance<ComplexData>();//.ToString();
                //var dc = sn.ToString();
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);


            var a0 = masterItem.CreateSnapshot();
            var b0 = masterItem.CreateSnapshot();

            var r = a0.GetResults(b0, "").ToList();
            r = r;
           // dc = dc;
            
            var masterSnaphot = masterItem.CreateSnapshot();
            var replicationMatrix = masterSnaphot.ToString();
            var clonedSnapshot = replicationMatrix.CreateSnapshot();
            var clonedItem = clonedSnapshot.CreateInstance();
            var lastSnapshot = clonedItem.CreateSnapshot();
            var results = masterSnaphot.GetResults(lastSnapshot, "").ToList();
            results = results;


            var s0 = masterItem.CreateSnapshot();
            //var s1 = y.CreateSnapshot();
            //var results0 = s0.GetResults(s1, "").ToList();
            //results0 = results0;
        }
    }
}
 

