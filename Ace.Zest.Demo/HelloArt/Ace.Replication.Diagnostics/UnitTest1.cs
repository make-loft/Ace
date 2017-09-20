using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AceSerialization;
using AceSerialization.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AceReplication.Diagnostics
{
    public static class Program
    {

        public static void CreateAndSerializeSnapshot()
        {
            var person0 = DiagnosticsGraph.Create();
            var snapshot0 = person0.CreateSnapshot();
            string rawSnapsot0 = snapshot0.ToString();
            Console.WriteLine(rawSnapsot0);
            Console.ReadKey();
        }

        public static void UseClassicalJsonSettings()
        {
            Snapshot.DefaultReplicationProfile.AttachId = false;
            Snapshot.DefaultReplicationProfile.AttachType = false;
            Snapshot.DefaultReplicationProfile.SimplifySets = true;
            Snapshot.DefaultReplicationProfile.SimplifyMaps = true;
            
            Snapshot.DefaultKeepProfile.SimplexConverter.AppendTypeInfo = false;
            Snapshot.DefaultKeepProfile.SimplexConverter.Converters
                .OfType<NumberConverter>().First().AppendSyffixes = false;   
        }

        public static void CreateAndSerializeSnapshotToClassicJsonStyle()
        {
            
            var person0 = DiagnosticsGraph.Create();
            var snapshot0 = person0.CreateSnapshot();
            string rawSnapsot0 = snapshot0.ToString();
            Console.WriteLine(rawSnapsot0);
            var person0A = rawSnapsot0.ParseSnapshot().ReplicateGraph<Person>();
            Console.WriteLine(person0A.FirstName);
            Console.ReadKey();
        }

        public static void Replicate()
        {   
            var person0 = DiagnosticsGraph.Create();

            var ppp = person0.MemberwiseClone(true);
            var snapshot0 = person0.CreateSnapshot();
            var person1 = snapshot0.ReplicateGraph<Person>();
            
            Console.WriteLine(person0.Roles[1].Name); // old graph value: Thomas Anderson
            Console.WriteLine(person1.Roles[1].Name); // new graph value: Thomas Anderson
            person1.Roles[1].Name = "Agent Smith";
            
            Console.WriteLine(person0.Roles[1].Name); // old graph value: Thomas Anderson
            Console.WriteLine(person1.Roles[1].Name); // new graph value: Agent Smith
            Console.ReadKey(); // result: both graphs are isolated from each other
        }
        
        public static void Reconstract()
        {   
            var person0 = DiagnosticsGraph.Create();
            
            var cache = new Dictionary<object, int>();
            var s = person0.CreateSnapshot(null, null, cache);
            
            Console.WriteLine(person0.Roles[1].Name); // old graph value: Thomas Anderson
            Console.WriteLine(person0.FirstName); // old graph value: Keanu
            person0.Roles[1].Name = "Agent Smith";
            person0.FirstName = "Zion";
            person0.Roles.RemoveAt(0);

            var person1 = (Person)s.ReconstructGraph(cache);
         
            Console.WriteLine(person0.Roles[1].Name); // old graph value: Thomas Anderson
            Console.WriteLine(person1.Roles[1].Name); // old graph value: Thomas Anderson
            Console.WriteLine(person0.FirstName); // old graph value: Keanu
            Console.WriteLine(person1.FirstName); // old graph value: Keanu
            Console.ReadKey(); // result: person0 & person1 is the same one graph
        }

        public static void Justapose()
        {
            Snapshot.DefaultReplicationProfile.AttachId = false;
            Snapshot.DefaultReplicationProfile.AttachType = false;
            Snapshot.DefaultReplicationProfile.SimplifySets = true;
            Snapshot.DefaultReplicationProfile.SimplifyMaps = true;

            var person0 = DiagnosticsGraph.Create();
            var person1 = DiagnosticsGraph.Create();
            
            person0.Roles[1].Name = "Agent Smith";
            person0.FirstName = "Zion";
            
            var snapshot0 = person0.CreateSnapshot();
            var snapshot1 = person1.CreateSnapshot();
            
            var results = snapshot0.Juxtapose(snapshot1);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Console.ReadKey();
        }
        
        public class MyClassA
        {
            private int AF { get; set; }
        }
        
        public class MyClassB : MyClassA
        {
            public int BF { get; set; }
        }
        
        public static void Main()
        {
            Console.WriteLine(double.Parse("     123  "));
            Console.ReadKey();
            var f = CultureInfo.CurrentCulture.NumberFormat;
            Console.WriteLine(f);
            Console.WriteLine(123.0.ToString());
            Console.WriteLine(123.012345.ToString("N"));
            var d = double.Parse("123.");
            d = d;
            var r = typeof(MyClassB).GetProperty("BF", BindingFlags.NonPublic|BindingFlags.Instance);

            Console.WriteLine(r?.Name);
            r = r;
           // new UnitTest1().TestMethod1();
            var xi = "abc/\"".CreateSnapshot().ToString();
            Console.WriteLine(xi);
            var a = new ComplexData();
            //a.Ha.Method
            var pp = a.Ptr;
            var b = a.MemberwiseClone(true);
            var aa = a.CreateSnapshot();
            var s = aa.ToString().ParseSnapshot();
            var c = aa.ReplicateGraph();
            Replicate();
            var rr = new [] {"abc"}.Aggregate((x, y) => x + Environment.NewLine + y);
            
            Justapose();
            Reconstract();
            Replicate();
            CreateAndSerializeSnapshotToClassicJsonStyle();

            Console.ReadKey();
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
        //[DataMember]
        //public XElement Element = new XElement("qwe");
        
        [DataMember]
        public IntPtr Ptr = new IntPtr(123);
        
        [DataMember]
        public Action Ha = () => { };

        [DataMember] public object[,,] Arr =
            {{{"abc", "x1", "asd"}, {"t", "d", "qwerty"}}, {{"7abc", "7x1", "7asd"}, {"8t", "8d", "8qwerty"}}};
        
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

        //[DataMember]
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
            var clonedItem = clonedSnapshot.ReplicateGraph<ComplexData>();
            var lastSnapshot = clonedItem.CreateSnapshot();
            var results = masterSnaphot.Juxtapose(lastSnapshot, "").ToList();
            results = results;
        }

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
                var x = Newtonsoft.Json.JsonConvert.SerializeObject(masterItem, settings);
                var y = Newtonsoft.Json.JsonConvert.DeserializeObject<ComplexData>(x);
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
                var dc = xx.CreateSnapshot().ReplicateGraph<ComplexData>();//.ToString();
                //var dc = sn.ToString();
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);


            var a0 = masterItem.CreateSnapshot();
            var b0 = masterItem.CreateSnapshot();

            var r = a0.Juxtapose(b0, "").ToList();
            r = r;
           // dc = dc;
            
            var masterSnaphot = masterItem.CreateSnapshot();
            var replicationMatrix = masterSnaphot.ToString();
            var clonedSnapshot = replicationMatrix.CreateSnapshot();
            var clonedItem = clonedSnapshot.ReplicateGraph();
            var lastSnapshot = clonedItem.CreateSnapshot();
            var results = masterSnaphot.Juxtapose(lastSnapshot, "").ToList();
            results = results;


            var s0 = masterItem.CreateSnapshot();
            //var s1 = y.CreateSnapshot();
            //var results0 = s0.GetResults(s1, "").ToList();
            //results0 = results0;
        }
    }
}
 

