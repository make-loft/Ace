using System;

#if XAMARIN
using Xamarin.Forms;
#else
using System.ComponentModel;
#endif

namespace Ace.Markup
{
	public class TypeExtension : Patterns.AMarkupExtension
	{
		public TypeExtension() => Key = null;
		
		public TypeExtension(Type key) => Key = key;
		
		[TypeConverter(typeof(TypeTypeConverter))]
		public Type Key { get; set; }

		public override object Provide(object targetObject, object targetProperty = null) => Key;
	}
}