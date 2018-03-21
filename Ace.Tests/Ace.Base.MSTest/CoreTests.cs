using System.Linq;
using System.Reflection;
using Ace.Base.MSTest.PatternMatching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest
{
	[TestClass] public class CoreTests
	{
		[TestMethod] public void TestIsMatching() => IsMatching.Test();
		[TestMethod] public void TestSwithLikeMatching() => SwithLikeMatching.Test();
		[TestMethod] public void TestLambdaStyledPatternMatching() => LambdaStyledMatching.Test();

		public void RunAll() => GetType().GetRuntimeMethods().Where(m => m.Name.StartsWith("Test"))
			.ForEach(m => m.Invoke(this, null));
	}
}