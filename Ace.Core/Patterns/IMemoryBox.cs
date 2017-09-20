using System;

namespace Ace.Patterns
{
    public interface IMemoryBox
    {
        bool Check<TItem>(string key = null);
        void Destroy<TItem>(string key = null);
        void Keep<TItem>(TItem item, string key = null);
        object Revive(string key, Type type, params object[] constructorArgs);
    }
}