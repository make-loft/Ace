using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
{
	public static class ToExtension
	{
		public static void Test()
		{
			var line0 = new Line();
			line0.To(out var line1);
			line0.To(out Shape shape);
			
			Assert.AreEqual(line0, line1);
			Assert.AreEqual(line0, shape);

			int i = 3;
			long l = i;
			i.To(out var j);
			i.To(out long m);
			
			Assert.AreEqual(i, j);
			Assert.AreEqual(l, m);
		}
	}
}