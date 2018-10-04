using Ace;
using Ace.Converters;

namespace System
{
    internal class Enum
    {
        private static readonly object[] Modifiers =
            {Modifier.Original, Modifier.RemoveUnderlines, Modifier.ToLower, Modifier.ToUpper};

        internal static object[] GetValues(Type type) =>
            typeof(Modifier).Is(type) ? Modifiers : throw new ArgumentException();
    }
}