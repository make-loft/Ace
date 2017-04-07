namespace Art.Patterns
{
    public interface IMemoryBox
    {
        bool Check<TItem>(string key = null);
        void Destroy<TItem>(string key = null);
        void Keep<TItem>(TItem item, string key = null);
        TItem Revive<TItem>(string key = null, params object[] constructorArgs);
    }
}