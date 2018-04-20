using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
{
	public static class IsMatching
	{
		public static void Test()
		{	
			var line = new Line();
			var circle = new Circle();
			
			TestIs(line);
			TestIs(line, circle);

			TestIs((Line) null);
			TestIs((Line) null, (Circle) null);
			TestIs(line, (Circle) null);
			TestIs((Line) null, circle);
			
			Assert.IsTrue(1.To(out int? nl).Is());
			Assert.AreEqual(1, nl);
			TestIsNull();
			TestIsNot();
		}

		private static void TestIs<TShapeA>(TShapeA shapeA)
			where TShapeA : Shape
		{
			var shapeALikeShape = (Shape) shapeA;
		
			Assert.AreEqual(shapeALikeShape.Is(), shapeA.Is());
			Assert.AreEqual(shapeALikeShape.IsNull(), shapeA.IsNull());
			Assert.AreEqual(shapeALikeShape != null, shapeALikeShape.Is());
			Assert.AreEqual(shapeALikeShape == null, shapeALikeShape.IsNull());
			
			Assert.AreEqual(true, shapeA.Is(shapeA));
			Assert.AreEqual(true, shapeA.Is<Shape>(shapeA));
			Assert.AreEqual(true, shapeALikeShape.Is<Shape>(shapeA));
			Assert.AreEqual(shapeALikeShape.Is(), shapeALikeShape.Is<TShapeA>(shapeA));
		}
		
		private static void TestIs<TShapeA, TShapeB>(TShapeA shapeA, TShapeB shapeB)
			where TShapeA : Shape where TShapeB : Shape
		{
			var shapeALikeShape = (Shape) shapeA;
			var shapeBLikeShape = (Shape) shapeB;

			var isNulls = shapeA == null && shapeB == null;
			
			Assert.AreEqual(false, shapeA.Is<TShapeB>(shapeB));
			Assert.AreEqual(isNulls, shapeA.Is<Shape>(shapeB));
			Assert.AreEqual(isNulls, shapeA.Is<Shape>(shapeBLikeShape));
			
			Assert.AreEqual(false, shapeALikeShape.Is<TShapeB>(shapeB));
			Assert.AreEqual(isNulls, shapeALikeShape.Is<Shape>(shapeB));
			Assert.AreEqual(isNulls, shapeALikeShape.Is(shapeBLikeShape));
		}

		private static void TestIsNull()
		{
			Assert.IsTrue(Const.Null.IsNull());
			Assert.IsTrue(Const.Null.To(out int? _).IsNull());
			
			Assert.IsTrue(Const.Null.IsNull(out var nl));
			Assert.AreEqual(null, nl);
			
			Assert.IsFalse(new object().IsNull(out var x, 9));
			Assert.AreEqual(x, 9);
		}
		
		private static void TestIsNot()
		{
			new object().To(out var a);
			new object().To(out var b);
			
			Assert.IsTrue(a.Is(a));
			Assert.IsTrue(b.Is(b));
			Assert.IsFalse(a.Is(b));
			
			var mA = new ModelA();
			var mB = new ModelB();
			var mALikeObject = (object) mA;

			Assert.IsTrue(mA.Is(mALikeObject));
			Assert.IsFalse(mA.Is<ModelA>(mB));
			Assert.IsFalse(mA.Is<ModelB>(mB));
			Assert.IsTrue(mALikeObject.Is(mALikeObject));
			
			Assert.IsFalse(mA.IsNot(mALikeObject));
			Assert.IsTrue(mA.IsNot<ModelA>(mB));
			Assert.IsTrue(mA.IsNot<ModelB>(mB));
			Assert.IsFalse(mALikeObject.IsNot(mALikeObject));

			5.To(out int? i);
			Assert.IsTrue(i.Is(5));
			Assert.IsFalse(i.Is(7));
			Assert.IsTrue(i.IsNot(7));
			Assert.IsTrue(7.IsNot(i));
			Assert.IsFalse(i.IsNot(5));
			Assert.IsFalse(5.IsNot(i));
		}
	}
}