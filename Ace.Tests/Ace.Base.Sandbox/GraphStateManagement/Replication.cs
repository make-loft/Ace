using System;
using System.Linq;
using Ace.Replication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.GraphStateManagement
{
    public static class Replication
    {
        public static void Test()
        {
            var person0 = DiagnosticsGraph.Create(DateTime.Now);
            
            var rp = new ReplicationProfile {AttachId = true};
            var kp = Snapshot.DefaultKeepProfile;
            
            var snapshot0 = person0.CreateSnapshot(rp, kp);
            var person1 = snapshot0.ReplicateGraph<Person>();
            var snapshot1 = person1.CreateSnapshot(rp, kp);

            Assert.IsTrue(snapshot0.JuxtaposeLikeEtalon(snapshot1).All(j => j.State == Etalon.State.Identical));

            var sourceName = person1.Roles[1].Name;
            person1.Roles[1].Name = "Agent Smith";
            Assert.AreEqual(person0.Roles[1].Name, sourceName);
            Assert.AreEqual(person1.Roles[1].Name, "Agent Smith");
        }
    }
}