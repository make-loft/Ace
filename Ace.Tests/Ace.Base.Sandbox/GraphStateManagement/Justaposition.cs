using System;
using System.Linq;
using Ace.Replication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.GraphStateManagement
{
    public static class Justaposition
    {
        public static void Test()
        {
            var rp = new ReplicationProfile
            {
                // set this settings for less details into output
                AttachId = true,
                AttachType = false,
                SimplifyMaps = true,
                SimplifySets = true
            };
            var kp = Snapshot.DefaultKeepProfile;


            var timestamp = DateTime.Now;
            var person0 = DiagnosticsGraph.Create(timestamp);
            var person1 = DiagnosticsGraph.Create(timestamp);

            person1.Roles[1].Name = "Agent Smith";
            person1.FirstName = "Zion";

            var snapshot0 = person0.CreateSnapshot(rp, kp);
            var snapshot1 = person1.CreateSnapshot(rp, kp);

            var results = snapshot0.JuxtaposeLikeEtalon(snapshot1).ToArray();

            Assert.AreEqual(results.Count(j => j.State != Etalon.State.Identical), 2);
            Assert.AreEqual(results.First(j => j.State != Etalon.State.Identical).Sample, "Zion");
            Assert.AreEqual(results.Last(j => j.State != Etalon.State.Identical).Sample, "Agent Smith");
        }
    }
}