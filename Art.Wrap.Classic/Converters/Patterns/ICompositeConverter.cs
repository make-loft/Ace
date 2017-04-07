using System.Windows.Data;

namespace Art.Converters.Patterns
{
    public interface ICompositeConverter : IValueConverter
    {
        IValueConverter PostConverter { get; set; }
        object PostConverterParameter { get; set; }
    }
}