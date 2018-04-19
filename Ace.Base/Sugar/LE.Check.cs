using System.Linq;
using IBools = System.Collections.Generic.IEnumerable<bool>;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static bool[] Check<T>(this T o, params bool[] pattern) => pattern;

		public static bool All(this IBools pattern, bool value) => pattern.All(value ? Const.IsTrue : Const.IsFalse);
		public static bool Any(this IBools pattern, bool value) => pattern.Any(value ? Const.IsTrue : Const.IsFalse);
		public static int Count(this IBools pattern, bool value) => pattern.Count(value ? Const.IsTrue : Const.IsFalse);
	}
}