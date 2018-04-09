using System;
using System.Linq;
using Ace.Replication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest.Juxtaposition
{
    public static class JuxtapositionExtensions
    {
        public static void Test() => Justapose();
        
        private static void Justapose()
        {
            // set this settings for less details into output
            Snapshot.DefaultReplicationProfile.AttachId = false;
            Snapshot.DefaultReplicationProfile.AttachType = false;
            Snapshot.DefaultReplicationProfile.SimplifySets = true;
            Snapshot.DefaultReplicationProfile.SimplifyMaps = true;

            var timestamp = DateTime.Now;
            var person0 = DiagnosticsGraph.Create(timestamp);
            var person1 = DiagnosticsGraph.Create(timestamp);
            
            person1.Roles[1].Name = "Agent Smith";
            person1.FirstName = "Zion";
            
            var snapshot0 = person0.CreateSnapshot();
            var snapshot1 = person1.CreateSnapshot();
            
            var results = snapshot0.JuxtaposeLikeEtalon(snapshot1).ToArray();

            Assert.AreEqual(results.Count(j => j.State != Etalon.State.Identical), 2);
            Assert.AreEqual(results.First(j => j.State != Etalon.State.Identical).Sample, "Zion");
            Assert.AreEqual(results.Last(j => j.State != Etalon.State.Identical).Sample, "Agent Smith");           
        }
    }
}