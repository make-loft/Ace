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
    }
}