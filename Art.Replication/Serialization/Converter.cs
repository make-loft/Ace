using System.Globalization;

namespace Art.Serialization
{
    public class Converter
    {              
        public static readonly object NotParsed = new object();
        public CultureInfo ActiveCulture = CultureInfo.InvariantCulture;

        public virtual string Convert(object value) => value?.ToString();
        public virtual object Revert(string value, string typeCode) => value;               
    }
}
