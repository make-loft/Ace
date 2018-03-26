using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest.PatternMatching
{
	public static class SwithLikeMatching
	{
		public static void Test()
		{
			TestTypeMatching();
			TestValueMatching();
		}

		private static void TestTypeMatching()
		{
			new ModelA().To(out var modelA);
			new ModelB().To(out var modelB);
			new ModelC().To(out var modelC);
			Assert.AreEqual($"A {modelA}", Match(modelA));
			Assert.AreEqual("B", Match(modelB));
			Assert.AreEqual($"C {modelC}", Match(modelC));
			Assert.AreEqual("null", Match((IModel) null));
		}

		private static string Match(IModel model) =>
			model.Match().Is(out var s) &&

			s.Case(out ModelA a) ? $"A {a}" :	// s.Case<ModelA>(out var a) ? $"A {a}" :
			s.Case(out ModelB _) ? "B" :		// s.Case<ModelB>() ? "B" :
			s.Case(out var c) ? $"C {c}" :		// s.Case(out var c) ? $"C {c}" :
			s.Case(null) ? "null" :

			throw new ArgumentException();

		private static void TestValueMatching()
		{
			Assert.AreEqual("X=null, Y=null", Match(new Point {X = null, Y = null}));
			Assert.AreEqual("X=null, Y=1234", Match(new Point {X = null, Y = 1234}));
			Assert.AreEqual("X=4321, Y=null", Match(new Point {X = 4321, Y = null}));
			Assert.AreEqual("X=4321, Y=1234", Match(new Point {X = 4321, Y = 1234}));
			Assert.AreEqual("X=4321, Y=Bingo", Match(new Point {X = 4321, Y = 777}));
		}

		private static string Match(Point point) =>
			point.Match(
				point.X.To(out object x),
				point.Y.To(out object y)
			).Is(out var s) &&

			s.Case(null, null) ? "X=null, Y=null" :
			s.Case(null, y) ? $"X=null, Y={y}" :
			s.Case(x, null) ? $"X={x}, Y=null" :
			s.Case(x, 777) ? $"X={x}, Y=Bingo" :
			s.Case(x, y) ? $"X={x}, Y={y}" :

			throw new ArgumentException();
	}
}