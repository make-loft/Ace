﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Art.Replication.Diagnostics
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var t = typeof(DateTimeOffset).IsPrimitive;
        }
    }
}
