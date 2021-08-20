using System;
using Ace.Adapters;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
#endif

namespace Ace.Markup
{
	public class Store : Patterns.AMarkupExtension
	{
		public Store() => Key = default;

		public Store(Type key) => Key = key;

		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Key { get; set; }

		public override object Provide(object targetObject, object targetProperty = null) =>
			RoutedCommandsAdapter.SetCommandBindings(targetObject, Ace.Store.Get(Key));
	}
}