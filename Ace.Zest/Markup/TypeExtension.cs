using System;
using Xamarin.Forms;

namespace Ace.Markup
{
    public class TypeExtension : Patterns.AMarkupExtension
    {
        [TypeConverter(typeof(TypeTypeConverter))]
        public Type Key { get; set; }

        public override object Provide(object targetObject, object targetProperty = null) => Key;
    }
}
