using Ace.Base.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
{
	public static class WithExtension
	{
		public static void Test()
		{
			3D.To(out var w);
			4D.To(out var h);
			new Rectangle().To(out var r).With(r.Width = w, r.Height = h);
			Assert.AreEqual(w, r.Width);
			Assert.AreEqual(h, r.Height);
		}
	}
}