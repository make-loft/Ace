using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest.PatternMatching
{
    public static class LambdaStyledMatching
    {
        public static void Test()
        {
            var c = new Circle {Radius = 9};
            var r = new Rectangle {Widh = 3, Heidth = 4};
            var t = new Triangle();
            Assert.AreEqual(0d, new Line().CalculateSquare());
            Assert.AreEqual(Math.PI * c.Radius * c.Radius, c.CalculateSquare());
            Assert.AreEqual(r.Widh * r.Heidth, r.CalculateSquare());
			
            try
            {
                t.CalculateSquare();
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual($"Undefined case for '{t}'", e.Message);
            }
        }

        private static double CalculateSquare(this Shape shape) =>
            shape.Match(
                (Line _) => 0,
                (Circle c) => Math.PI * c.Radius * c.Radius,
                (Rectangle r) => r.Widh * r.Heidth
            );
    }
}