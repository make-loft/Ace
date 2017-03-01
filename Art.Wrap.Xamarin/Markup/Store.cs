using System;
using Xamarin.Forms.Xaml;

namespace Aero.Markup
{
    public class Store : IMarkupExtension
    {
        public Type Key { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var itemType = Key;
            var methodInfo = typeof (Aero.Store).GetMethod("Get").
                MakeGenericMethod(itemType.DeclaringType ?? itemType);
            var item = methodInfo.Invoke(null, new object[] {new object[0]});
            return item;
        }
    }
}