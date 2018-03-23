using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest.Sugar
{
	public static class NewExtension
	{
		public static void Test()
		{
			TestArray();
			TestList();
		}
		
		private static void TestArray()
		{
			var array = New.Array(1, 2, 3, 4, 5);
			var names = New.Array("Abc", "Xyz", "Make");
			
			Assert.IsTrue(array is int[]);
			Assert.IsTrue(names is string[]);

			Assert.AreEqual(array.Aggregate(0, (i, j) => i + j), 15);
			Assert.AreEqual(names.Aggregate("", (i, j) => i + j), "AbcXyzMake");
		}
		
		private static void TestList()
		{
			var array = New.List(1, 2, 3, 4, 5);
			var names = New.List("Abc", "Xyz", "Make");
			
			Assert.IsTrue(array is List<int>);
			Assert.IsTrue(names is List<string>);

			Assert.AreEqual(array.Aggregate(0, (i, j) => i + j), 15);
			Assert.AreEqual(names.Aggregate("", (i, j) => i + j), "AbcXyzMake");
		}
	}
}