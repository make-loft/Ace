using System;
using System.ComponentModel;
using System.Windows.Data;

namespace Art.Markup
{
    public class StoreBinding : Binding
    {
        [TypeConverter(typeof(XamlTypeConverter))]
        public Type StoreKey
        {
            get => Source?.GetType();
            set
            {
                var itemType = value;
                var methodInfo = typeof(Art.Store).GetMethod("Get").
                    MakeGenericMethod(itemType.DeclaringType ?? itemType);
                Source = methodInfo.Invoke(null, new object[] { new object[0] });
            }
        }
    }
}
