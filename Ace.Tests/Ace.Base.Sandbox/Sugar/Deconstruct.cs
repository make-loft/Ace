using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
{
    public static class DeconstructExtensions
    {
        public static void Test()
        {
            TestDeconstruct0();
            TestDeconstruct1();
            
            // var x = 0;
            // var x = (0);
            // var (x) = 0;  // [CS1001] Identifier expected
            // var (x) = (0); // [CS1001] Identifier expected
				
            // var (x, y, z) = 0;
            // var ((x, y, z), t) = (0, 7);
            // var ((x, y, z), (t)) = (0, 7); // [CS1001] Identifier expected
            // (var (x, y, z), var t) = (0, 7);				
            // (var (x, y, z), var (t)) = (0, 7); // [CS1001] Identifier expected

            // var ((x, y, z), (t0, t1)) = (0, 7);
            // (var (x, y, z), var (t0, t1)) = (0, 7);
            // (var (x, y, z) = 0, var (t0, t1) = 7); // [CS8185] A declaration is not allowed in this context.

            // 0.To(out var (x, y, z)); // [CS8199] The syntax 'var (...)' as an lvalue is reserved.

            // System.Console.WriteLine($"({x}, {y}, {z}), {t}"); // (0, 0, 0), 7
            // System.Console.WriteLine($"({x}, {y}, {z}), ({t0}, {t1})"); // (0, 0, 0), (7, 7)
        }
        
        private static void TestDeconstruct0()
        {
            var (x, y, z) = 1;
            Assert.AreEqual(x, 1);
            Assert.AreEqual(y, 1);
            Assert.AreEqual(z, 1);
			
            (x, y, z) = 0;
            Assert.AreEqual(x, 0);
            Assert.AreEqual(y, 0);
            Assert.AreEqual(z, 0);
        }

        private static void TestDeconstruct1()
        {
            var ((x, y, z), t, n) = (1, 5, "xyz");
            Assert.AreEqual(x, 1);
            Assert.AreEqual(y, 1);
            Assert.AreEqual(z, 1);
            Assert.AreEqual(t, 5);
            Assert.AreEqual(n, "xyz");

            ((x, y, z), t, n) = (0, 7, "zyx");
            Assert.AreEqual(x, 0);
            Assert.AreEqual(y, 0);
            Assert.AreEqual(z, 0);
            Assert.AreEqual(t, 7);
            Assert.AreEqual(n, "zyx");
        }
    }
}