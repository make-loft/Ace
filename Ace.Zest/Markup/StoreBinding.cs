// ReSharper disable RedundantUsingDirective
using System;
using System.ComponentModel;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;

namespace Ace.Markup
{
    public class StoreBinding : System.Windows.Data.Binding
    {
        [TypeConverter(typeof(XamlTypeConverter))]
        public Type StoreKey
        {
            get => Source?.GetType();
            set => Source = Ace.Store.Get(value);
        }
    }
}