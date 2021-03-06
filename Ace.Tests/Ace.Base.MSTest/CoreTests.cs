using System.Linq;
using System.Reflection;
using Ace.Base.Sandbox.GraphStateManagement;
using Ace.Base.Sandbox.Sugar;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest
{
	[TestClass] public class CoreTests
	{
		[TestMethod] public void TestNewExtension() => NewExtensions.Test();
		[TestMethod] public void TestToExtension() => Basics.Test();
		[TestMethod] public void TestWithLikeExtension() => WithLikeExtensions.Test();
		[TestMethod] public void TestDeconstructExtension() => DeconstructExtensions.Test();
		
		[TestMethod] public void TestJuxtoposition() => Justaposition.Test();
		[TestMethod] public void TestReconstruction() => Reconstruction.Test();
		[TestMethod] public void TestAntidistortion() => Antidistortion.Test();
		[TestMethod] public void TestReplication() => Sandbox.GraphStateManagement.Replication.Test();
		
		[TestMethod] public void TestIsMatching() => IsMatching.Test();
		[TestMethod] public void TestCombinedMatching() => CombinedMatching.Test();
		[TestMethod] public void TestSwithLikeMatching() => SwithLikeMatching.Test();
		[TestMethod] public void TestLambdaStyledPatternMatching() => LambdaStyledMatching.Test();

		public void RunAll() => GetType().GetRuntimeMethods().Where(m => m.Name.StartsWith("Test"))
			.ForEach(m => m.Invoke(this, null));
	}
}