using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
{
	public static class Basics
	{
		public static void Test()
		{
			var line0 = new Line();
			line0.To(out var line1);
			line0.To(out Shape shape);
			
			Assert.AreSame(line0, line1);
			Assert.AreSame(line0, shape);

			int i = 3;
			long l = i;
			i.To(out var j);
			i.To(out long m);
			
			Assert.AreEqual(i, j);
			Assert.AreEqual(l, m);
			
			Assert.AreEqual(i.As(), i);
			Assert.AreEqual(i.As(out j), i);
			Assert.AreEqual(j, i);
			
			Assert.IsTrue(TestPutUseChain());

			TestPut();
			TestUse();
		}

		private static bool TestPutUseChain() => int.TryParse("123", out var i).Put(i).Use(Console.WriteLine) == 123;
		
		public static void TestPut()
		{
			var a = new object();
			var b = new object();
			Assert.AreSame(a.Put(b), b);
			Assert.AreSame(b.Put(a), a);
			Assert.AreSame(a.Put(ref b), b);
			Assert.AreSame(b.Put(ref a), a);
		}

		private static void TestUse()
		{
			Assert.AreEqual(123.Use(out var x, 1).ToString(), "123");
			Assert.AreEqual(x, 1);
			
			Assert.AreEqual(2.Use(o => { x += o; }), 2);
			Assert.AreEqual(x, 3);
			
			Assert.AreEqual(2.Use(o => x += o), 2);
			Assert.AreEqual(x, 5);
			
			Assert.AreEqual(2.Use(() => x += 3), 2);
			Assert.AreEqual(x, 8);
			
			Assert.AreEqual(2.Call(() => { x += 4; }), 2);
			Assert.AreEqual(x, 12);
		}
	}
}