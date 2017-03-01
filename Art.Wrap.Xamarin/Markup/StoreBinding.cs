using System;
using System.Windows.Data;

namespace Aero.Markup
{
    public class StoreBinding : Binding
    {
        public Type StoreKey
        {
            get { return Source == null ? null : Source.GetType(); }
            set
            {
                var itemType = value;
                var methodInfo = typeof(Aero.Store).GetMethod("Get").
                    MakeGenericMethod(itemType.DeclaringType ?? itemType);
                Source = methodInfo.Invoke(null, new object[] { new object[0] });
            }
        }
    }
}
