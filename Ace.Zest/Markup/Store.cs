using System;
using Ace;
using Ace.Adapters;
using Ace.Specific;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
#endif

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

		static Store() => Memory.ActiveBox = Memory.ActiveBox ?? new Memory(new KeyFileStorage());
	}
}