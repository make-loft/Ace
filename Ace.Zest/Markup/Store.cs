// ReSharper disable RedundantUsingDirective
using System;
using System.ComponentModel;
using Ace.Adapters;
using TypeConverter = Xamarin.Forms.TypeConverterAttribute;
using TypeTypeConverter = Xamarin.Forms.TypeTypeConverter;

namespace Ace.Markup
{
	public class Store : Patterns.AMarkupExtension
	{
		public Store() => Key = null;

		public Store(Type key) => Key = key;

		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Key { get; set; }

		public override object Provide(object targetObject, object targetProperty = null) =>
			RoutedCommandsAdapter.SetCommandBindings(targetObject, Ace.Store.Get(Key));
	}
}