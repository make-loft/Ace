using System.Linq;
using IBooleans = System.Collections.Generic.IEnumerable<bool>;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static bool[] Check<T>(this T o, params bool[] pattern) => pattern;

		public static bool All(this IBooleans pattern, bool value) => pattern.All(value ? CN.IsTrue : CN.IsFalse);
		public static bool Any(this IBooleans pattern, bool value) => pattern.Any(value ? CN.IsTrue : CN.IsFalse);
		public static int Count(this IBooleans pattern, bool value) => pattern.Count(value ? CN.IsTrue : CN.IsFalse);
	}
}