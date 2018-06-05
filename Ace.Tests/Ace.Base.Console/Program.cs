using System;
using Ace.Base.MSTest;
using static System.Console;

namespace Ace.Base.Console
{
	static class Program
	{
		class Point
		{
			private int X, Y, Z;

			public void Deconstruct(out int x, out int y, out int z)
			{
				x = X;
				y = Y;
				z = Z;
			}
		}
		static void Main(string[] args)
		{
			var p = new Point();
			var (x, y, z) = p;
			
			
			try
			{
				var tests = new CoreTests();
				tests.RunAll();
				WriteLine("Done");
			}
			catch (Exception e)
			{
				WriteLine(e);
				ReadKey(true);
			}
		}
	}
}