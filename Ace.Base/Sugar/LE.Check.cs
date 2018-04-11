using System.Linq;
// ReSharper disable once CheckNamespace
namespace Ace
{
    // ReSharper disable InconsistentNaming
    public static partial class LE
    {
        public static bool[] Check<T>(this T o, params bool[] pattern) => pattern;

        public static bool All(this bool[] pattern, bool value) => pattern.All(value ? CN.IsTrue : CN.IsFalse);
        public static bool Any(this bool[] pattern, bool value) => pattern.Any(value ? CN.IsTrue : CN.IsFalse);
        public static int Count(this bool[] pattern, bool value) => pattern.Count(value ? CN.IsTrue : CN.IsFalse);
    }
}