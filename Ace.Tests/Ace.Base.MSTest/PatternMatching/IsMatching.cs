using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest.PatternMatching
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
        }

        private static void TestIs<TShapeA>(TShapeA shapeALikeA)
            where TShapeA : Shape
        {
            var shapeALikeShape = (Shape) shapeALikeA;

            var isNull = shapeALikeA is null;
			
            Assert.AreEqual(true, shapeALikeA.Is(shapeALikeA));
            Assert.AreEqual(true, shapeALikeA.Is<Shape>(shapeALikeA));
            Assert.AreEqual(true, shapeALikeShape.Is<Shape>(shapeALikeA));
            Assert.AreEqual(!isNull, shapeALikeShape.Is<TShapeA>(shapeALikeA));
        }
		
        private static void TestIs<TShapeA, TShapeB>(TShapeA shapeALikeA, TShapeB shapeBLikeB)
            where TShapeA : Shape where TShapeB : Shape
        {
            var shapeALikeShape = (Shape) shapeALikeA;
            var shapeBLikeShape = (Shape) shapeBLikeB;

            var isNulls = shapeALikeA == null && shapeBLikeB == null;
			
            Assert.AreEqual(false, shapeALikeA.Is<TShapeB>(shapeBLikeB));
            Assert.AreEqual(isNulls, shapeALikeA.Is<Shape>(shapeBLikeB));
            Assert.AreEqual(isNulls, shapeALikeA.Is<Shape>(shapeBLikeShape));
			
            Assert.AreEqual(false, shapeALikeShape.Is<TShapeB>(shapeBLikeB));
            Assert.AreEqual(isNulls, shapeALikeShape.Is<Shape>(shapeBLikeB));
            Assert.AreEqual(isNulls, shapeALikeShape.Is(shapeBLikeShape));
        }
    }
}