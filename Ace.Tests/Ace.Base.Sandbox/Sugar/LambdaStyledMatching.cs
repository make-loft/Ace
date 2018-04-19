using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
{
	public static class LambdaStyledMatching
	{
		public static void Test()
		{
			var c = new Circle {Radius = 9};
			var r = new Rectangle {Width = 3, Height = 4};
			var t = new Triangle();
			Assert.AreEqual(0d, new Line().CalculateSquare());
			Assert.AreEqual(Math.PI * c.Radius * c.Radius, c.CalculateSquare());
			Assert.AreEqual(r.Width * r.Height, r.CalculateSquare());
			Assert.AreEqual(double.NaN, ((Shape) null).CalculateSquare());
			
			try
			{
				t.CalculateSquare();
			}
			catch (ArgumentException e)
			{
				Assert.AreEqual($"Undefined case for '{t}'", e.Message);
			}
		}

		public static double CalculateSquare(this Shape shape) =>
			shape.Match(
				(Line _) => 0,
				(Circle c) => Math.PI * c.Radius * c.Radius,
				(Rectangle r) => r.Width * r.Height,
				() => double.NaN
			);
	}
}