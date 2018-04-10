using System;
using System.Linq;
using Ace.Replication;
using Ace.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.GraphStateManagement
{
    public static class Antidistortion
    {
        public static void Test()
        {
            var etalons = New.Array<object>(
                Guid.NewGuid(),
                Guid.NewGuid().ToString(),
                DateTime.Now,
                DateTime.Now.ToString("O"),
                123,
                123L
            );

            var rp = new ReplicationProfile();
            var kp = new KeepProfile();
            var etalonsSnapshot = etalons.CreateSnapshot(rp, kp);
            var samples = etalonsSnapshot.ReplicateGraph<object[]>();
            Assert.IsTrue(samples[0] is Guid);
            Assert.IsTrue(samples[1] is string);
            Assert.IsTrue(samples[2] is DateTime);
            Assert.IsTrue(samples[3] is string);
            Assert.IsTrue(samples[4] is int);
            Assert.IsTrue(samples[5] is long);


            var samplesSnapshot = samples.CreateSnapshot();
            Assert.IsTrue(etalonsSnapshot.JuxtaposeLikeEtalon(samplesSnapshot)
                .All(j => j.State == Etalon.State.Identical));
        }
    }
}