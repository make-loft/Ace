// ReSharper disable once RedundantUsingDirective
using System;
using System.ComponentModel;
using Ace.Adapters;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;

namespace Ace.Markup
{
    public class Store : Patterns.AMarkupExtension
    {
        public Store() => Key = null;

        public Store(Type key) => Key = key;

        [TypeConverter(typeof(XamlTypeConverter))]
        public Type Key { get; set; }

        public override object Provide(object targetObject, object targetProperty = null) =>
            RoutedCommandsAdapter.SetCommandBindings(targetObject, Ace.Store.Get(Key));
    }
}