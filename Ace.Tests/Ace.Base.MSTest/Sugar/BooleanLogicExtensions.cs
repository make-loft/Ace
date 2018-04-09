using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest.Sugar
{
    public static class BooleanLogicExtensions
    {
        public static void Test()
        {
            if (int.TryParse("123", out var a) | int.TryParse("321", out var b))
            {
                Assert.AreEqual(123, a);
                Assert.AreEqual(321, b);
            }
            
            if (int.TryParse("123", out var c).Or(int.TryParse("321", out var d)))
            {
                Assert.AreEqual(123, c);
                Assert.AreEqual(321, d);
            }

            if (int.TryParse("abc", out var e).Xor(int.TryParse("321", out var f)))
            {
                Assert.AreEqual(0, e);
                Assert.AreEqual(321, d);
            }
            
            if (int.TryParse("abc", out var g).And(int.TryParse("321", out var h)))
            {
                
            }
            else
            {
                Assert.AreEqual(0, g);
                Assert.AreEqual(321, h);
            }
        }
    }
}