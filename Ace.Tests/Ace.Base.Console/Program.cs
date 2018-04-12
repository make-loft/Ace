using System;
using Ace.Base.MSTest;
using static System.Console;

namespace Ace.Base.Console
{
	static class Program
	{
		
		
		static void Main(string[] args)
		{
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