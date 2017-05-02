using System.Text;
using Art.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Art.Replication.Diagnostics
{
    [TestClass]
    public class EscapeTests
    {
        [TestMethod]
        public void TestCapture12()
        {
            var i = 1;
            var escapeProfile = new EscapeProfile();
            var source = "\"ab\"";
            var builder = new StringBuilder();
            EscapeProfile.AppendEscapedLiteral(builder, source, ref i,
                escapeProfile.VerbatimUnescapeChars, '\"', "\"", true);
            var s = builder.ToString();
            Assert.AreEqual("ab", s);
        }

        [TestMethod]
        public void TestCapture11()
        {
            var i = 2;
            var escapeProfile = new EscapeProfile();
            var source = "@\"a\"<bb> ,";
            var builder = new StringBuilder();
            EscapeProfile.AppendEscapedLiteral(builder, source, ref i,
                escapeProfile.VerbatimUnescapeChars, '\"', "\"", true);
            var s = builder.ToString();
            Assert.AreEqual("a", s);
        }

        [TestMethod]
        public void TestCapture10()
        {
            var i = 0;
            var escapeProfile = new EscapeProfile();
            var source = "http://makeloft.xyz/";
            var s = escapeProfile.AppendWithEscape(new StringBuilder(), source,
                escapeProfile.VerbatimEscapeChars, true).ToString();
            Assert.AreEqual(source, s);
        }

        [TestMethod]
        public void TestCapture0()
        {
            var i = 0;
            var escapeProfile = new EscapeProfile();
            var s = escapeProfile.CaptureSimplex("\"abc\"\"cde\", xyz", ref i);
            Assert.AreEqual(s.Count, 6);
            Assert.AreEqual(s[0], "\"");
            Assert.AreEqual(s[1], "abc");
            Assert.AreEqual(s[2], "\"");
        }

        [TestMethod]
        public void TestCapture()
        {
            var i = 0;
            var escapeProfile = new EscapeProfile();
            var s = escapeProfile.CaptureSimplex("\"abc\" ,", ref i);
            Assert.AreEqual(s.Count, 3);
            Assert.AreEqual(s[0], "\"");
            Assert.AreEqual(s[1], "abc");
            Assert.AreEqual(s[2], "\"");
        }

        [TestMethod]
        public void TestEmptyCapture()
        {
            var i = 0;
            var escapeProfile = new EscapeProfile();
            var s = escapeProfile.CaptureSimplex("\"\"", ref i);
            Assert.AreEqual(s.Count, 3);
            Assert.AreEqual(s[0], "\"");
            Assert.AreEqual(s[1], "");
            Assert.AreEqual(s[2], "\"");
        }

        [TestMethod]
        public void TestEmptyCaptureWithVerbatim()
        {
            var i = 0;
            var escapeProfile = new EscapeProfile();
            var s = escapeProfile.CaptureSimplex("@\"\"", ref i);
            Assert.AreEqual(s.Count, 3);
            Assert.AreEqual(s[0], "\"");
            Assert.AreEqual(s[1], "");
            Assert.AreEqual(s[2], "\"");

            //Assert.AreEqual(s.Count, 4);
            //Assert.AreEqual(s[0], "@");
            //Assert.AreEqual(s[1], "\"");
            //Assert.AreEqual(s[2], "");
            //Assert.AreEqual(s[3], "\"");
        }
    }
}
