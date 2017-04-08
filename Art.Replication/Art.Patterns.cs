namespace Art
{
    public interface IExposable
    {
        void Expose();
    }

    public interface IConverter<in TIn, out TOut>
    {
        TOut Convert(TIn value, params object[] args);
    }
}
