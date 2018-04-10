using System;
using System.Collections.Generic;
using Ace.Replication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.GraphStateManagement
{
    public static class Reconstruction
    {
        public static void Test()
        {
            var rp = new ReplicationProfile {AttachId = true};
            var kp = Snapshot.DefaultKeepProfile;
            
            var person0 = DiagnosticsGraph.Create(DateTime.Now);
            
            var cache = new Dictionary<object, int>();
            var s = person0.CreateSnapshot(rp, kp, cache);
            
            var etalonRoleName = person0.Roles[1].Name; // old graph value: Thomas Anderson
            var etalonPersonName = person0.FirstName; // old graph value: Keanu
            
            person0.Roles[1].Name = "Agent Smith";
            person0.FirstName = "Zion";
            
            Assert.AreNotEqual(person0.Roles[1].Name, etalonRoleName);
            Assert.AreNotEqual(person0.FirstName, etalonPersonName);
            
            person0.Roles.RemoveAt(0);

            var person1 = (Person)s.ReconstructGraph(cache);
            
            Assert.AreEqual(person0.Roles[1].Name, etalonRoleName);
            Assert.AreEqual(person0.FirstName, etalonPersonName);
            
            Assert.AreEqual(person1.Roles[1].Name, etalonRoleName);
            Assert.AreEqual(person1.FirstName, etalonPersonName);

            Assert.IsTrue(ReferenceEquals(person0, person1));
        }
    }
}